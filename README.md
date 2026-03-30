# procosys-library-api

Library REST-API for Project Completion System (ProCoSys)

## CI/CD

| Workflow | Trigger | Description |
|---|---|---|
| 🤖 Build and Test | Pull request | Runs unit tests in parallel |
| ✏️ Verify formatting | Pull request | Checks code formatting |
| 🗃️ Verify PR Title | Pull request | Enforces conventional commit titles |
| 🚀🔬 Deploy to Dev | Pull request, manual | Deploys to Radix dev environment |
| 🚀🧪 Deploy to Test | Push to main, manual | Deploys to Radix test environment |
| 🚀🏭 Deploy to Prod | Push to main | Deploys to Radix prod environment |
| 🔁🏭 Deploy to Prod (Rollback) | Manual with git ref | Rolls back prod to a specific commit |

> **Important:** Merging a PR to `main` automatically deploys to both **test** and **prod**.

## PR Flow

1. Create a branch from `main`
2. Open a pull request
3. CI runs tests, formatting checks, and PR title validation
4. Changes are deployed to the **dev** environment for verification
5. Get review approval and merge

## Production Flow

1. PR is merged to `main`
2. **Test** and **prod** environments are deployed automatically
3. If a rollback is needed, use the **Deploy to Prod (Rollback)** workflow with the target commit SHA

## Manual Deployment

- **Dev/Test**: Trigger the deploy workflow manually from the Actions tab
- **Prod rollback**: Run "Deploy to Prod (Rollback)" with a git ref (commit SHA, tag, or branch)
