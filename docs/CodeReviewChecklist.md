# Code Review Checklist

Use this checklist for every pull request. A reviewer does not need to write a long review, but each item should be considered before approval.

## General

- [ ] The PR has a clear title and explains what changed.
- [ ] The PR touches only files related to the assigned task.
- [ ] No unrelated formatting, generated files, local config, or IDE files are included.
- [ ] The code follows `docs/CodingStandard.md`.
- [ ] Names are clear and consistent with the existing domain language.

## Build And Tests

- [ ] The solution builds locally.
- [ ] Relevant tests were run and results are mentioned in the PR.
- [ ] New or changed business logic has tests where practical.
- [ ] Existing tests were not weakened, skipped, or deleted without explanation.

## Architecture

- [ ] UI code stays in XAML/code-behind and business logic stays in services/viewmodels.
- [ ] Database access goes through repositories/services.
- [ ] No production code depends on mock data.
- [ ] No new circular project references or unnecessary dependencies were introduced.
- [ ] Shared behavior is reused instead of duplicated.

## Data And Security

- [ ] SQL uses parameters for user input.
- [ ] Connection strings, passwords, tokens, and machine-specific values are not committed.
- [ ] User session and login behavior use the database-backed authenticated user.
- [ ] Error handling gives useful information without exposing secrets.

## UI And UX

- [ ] Navigation works from the expected entry point.
- [ ] The app still asks for login when it should.
- [ ] Controls are readable and do not overlap at common window sizes.
- [ ] User-facing messages are clear and consistent.

## Final Checks

- [ ] The reviewer can explain the main risk of the PR.
- [ ] The author resolved or answered review comments.
- [ ] The branch is up to date with the target branch before merge.
