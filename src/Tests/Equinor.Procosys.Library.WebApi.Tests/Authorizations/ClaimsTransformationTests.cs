using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Domain;
using Equinor.Procosys.Library.WebApi.Authorizations;
using Equinor.Procosys.Library.WebApi.Caches;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Library.WebApi.Tests.Authorizations
{
    [TestClass]
    public class ClaimsTransformationTests
    {
        private ClaimsTransformation _dut;
        private Guid Oid = new Guid("{0B627D64-8113-40E1-9394-60282FB6BB9F}");
        private ClaimsPrincipal _principalWithOid;
        private readonly string Plant1 = "Plant1";
        private readonly string Plant2 = "Plant2";
        private readonly string Permission_Plant1 = Permissions.LIBRARY_GENERAL_READ;
        private readonly string Permission_Plant2 = Permissions.LIBRARY_GENERAL_WRITE;
        private readonly string _tagReadPermission = "TAG/READ";
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IPlantCache> _plantCacheMock;

        [TestInitialize]
        public void Setup()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(p => p.Plant).Returns(Plant1);

            _plantCacheMock = new Mock<IPlantCache>();
            _plantCacheMock.Setup(p => p.IsValidPlantForUserAsync(Plant1, Oid)).Returns(Task.FromResult(true));
            _plantCacheMock.Setup(p => p.IsValidPlantForUserAsync(Plant2, Oid)).Returns(Task.FromResult(true));

            var permissionCacheMock = new Mock<IPermissionCache>();
            permissionCacheMock.Setup(p => p.GetPermissionsForUserAsync(Plant1, Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission_Plant1, _tagReadPermission}));

            permissionCacheMock.Setup(p => p.GetPermissionsForUserAsync(Plant2, Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission_Plant2, _tagReadPermission}));

            _principalWithOid = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimsExtensions.OidType, Oid.ToString()));
            _principalWithOid.AddIdentity(claimsIdentity);
            
            _dut = new ClaimsTransformation(_plantProviderMock.Object, _plantCacheMock.Object, permissionCacheMock.Object);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddRoleClaimsForLibraryPermissions()
        {
            var result = await _dut.TransformAsync(_principalWithOid);

            AssertRoleClaimsForPlant1(result.Claims);
        }
        
        [TestMethod]
        public async Task TransformAsync_Twice_ShouldNotDuplicateRoleClaimsForPermissions()
        {
            await _dut.TransformAsync(_principalWithOid);
            var result = await _dut.TransformAsync(_principalWithOid);

            AssertRoleClaimsForPlant1(result.Claims);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldNotAddRoleClaimsForOtherPermissions()
        {
            var result = await _dut.TransformAsync(_principalWithOid);

            var roleClaims = GetRoleClaims(result.Claims);
            Assert.AreEqual(1, roleClaims.Count);
            Assert.IsFalse(roleClaims.Any(r => r.Value == _tagReadPermission));
        }

        [TestMethod]
        public async Task TransformAsync_ShouldNotAddAnyClaims_ForPrincipalWithoutOid()
        {
            var result = await _dut.TransformAsync(new ClaimsPrincipal());

            Assert.AreEqual(0, GetRoleClaims(result.Claims).Count);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldNotAddAnyClaims_WhenNoPlantGiven()
        {
            _plantProviderMock.SetupGet(p => p.Plant).Returns((string)null);

            var result = await _dut.TransformAsync(_principalWithOid);

            Assert.AreEqual(0, GetRoleClaims(result.Claims).Count);
        }
                
        [TestMethod]
        public async Task TransformAsync_OnSecondPlant_ShouldClearAllClaimsForFirstPlant()
        {
            var result = await _dut.TransformAsync(_principalWithOid);
            AssertRoleClaimsForPlant1(result.Claims);

            _plantProviderMock.SetupGet(p => p.Plant).Returns(Plant2);
            result = await _dut.TransformAsync(_principalWithOid);

            var claims = GetRoleClaims(result.Claims);
            Assert.AreEqual(1, claims.Count);
            Assert.IsNotNull(claims.SingleOrDefault(r => r.Value == Permission_Plant2));
        }

        private static List<Claim> GetRoleClaims(IEnumerable<Claim> claims)
            => claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        private void AssertRoleClaimsForPlant1(IEnumerable<Claim> claims)
        {
            var roleClaims = GetRoleClaims(claims);
            Assert.AreEqual(1, roleClaims.Count);
            Assert.IsNotNull(roleClaims.SingleOrDefault(r => r.Value == Permission_Plant1));
        }
    }
}
