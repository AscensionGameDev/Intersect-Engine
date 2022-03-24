# Contributing to Intersect

## Links
- [Discord](https://discord.gg/CvvsVpPMuF)
- [Ascension Game Dev Community Forum](https://ascensiongamedev.com)

## Table of Contents
- [Contributing to Intersect](#contributing-to-intersect)
  - [Links](#links)
  - [Table of Contents](#table-of-contents)
  - [- Additional Resources](#--additional-resources)
  - [Code of Conduct](#code-of-conduct)
  - [Git, GitHub, and GPG](#git-github-and-gpg)
    - [Git and GitHub](#git-and-github)
      - [Cloning](#cloning)
      - [Example entry in `~/.ssh/config` if cloning with SSH](#example-entry-in-sshconfig-if-cloning-with-ssh)
    - [Commit Signing with GPG](#commit-signing-with-gpg)
  - [Commits and Pull Requests](#commits-and-pull-requests)
    - [Commit Message and Pull Request Title Quality](#commit-message-and-pull-request-title-quality)
    - [Commit Quality](#commit-quality)
    - [Pull Request Quality](#pull-request-quality)
      - [One Pull Request, One Issue](#one-pull-request-one-issue)
      - [Associating Issues](#associating-issues)
      - [Validation/Verification](#validationverification)
      - [Merge Conflicts](#merge-conflicts)
      - [Assets](#assets)
      - [Your First Merged Pull Request](#your-first-merged-pull-request)
      - [Pull Request Maintenance](#pull-request-maintenance)
    - [Merging Guidelines](#merging-guidelines)
      - [General Merging Guidelines](#general-merging-guidelines)
      - ["Promotion" Pull Requests](#promotion-pull-requests)
      - ["Additive"/"Subtractive" Pull Requests](#additivesubtractive-pull-requests)
    - [Additional Resources](#additional-resources)
--------------------

## Code of Conduct

We expect everyone to adhere to the [Code of Conduct](CODE_OF_CONDUCT.md), and maintainers to uphold the aforementioned Code of Conduct.

If we fail to do so, please let us know [on our Discord](https://discord.gg/CvvsVpPMuF) in our [#feedback channel](https://discord.com/channels/363106200243535872/951191802977849354) (requires accepting the rules when you join).

If you see possible violations of the Code of Conduct please report them per the instructions in our [#reporting channel on Discord](https://discord.com/channels/363106200243535872/951209899134709780).

--------------------
## Git, GitHub, and GPG

### Git and GitHub

[Set up Git (GitHub Docs)](https://docs.github.com/en/get-started/quickstart/set-up-git)

[GitHub Docs](https://docs.github.com/en)

[GitHub Cheat Sheet](https://github.com/tiimgreen/github-cheat-sheet/blob/master/README.md)

#### Cloning

We recommend [cloning with SSH](https://docs.github.com/en/github/getting-started-with-github/about-remote-repositories/#cloning-with-ssh-urls) which requires [SSH keys](https://docs.github.com/en/articles/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent).

#### Example entry in `~/.ssh/config` if cloning with SSH

```
Host github.com
    HostName github.com
    User git
    IdentityFile ~/.ssh/id_rsa
```

### Commit Signing with GPG

Submitting to [`main`](https://github.com/AscensionGameDev/Intersect-Engine/tree/main) _requires_ commits to be GPG signed, and GPG signing of all commits is strongly recommended for _all_ pull requests.

You can read about how to set up Git for commit signing [in the GitHub Docs](https://docs.github.com/en/authentication/managing-commit-signature-verification/signing-commits).

More information can be found in the [Verify commit signatures](https://docs.github.com/en/authentication/managing-commit-signature-verification) section on GitHub Docs.

--------------------

## Commits and Pull Requests

### Commit Message and Pull Request Title Quality

Commit message and pull request titles should clearly reflect what they are doing and should be formatted in lower case _except_ for names (classes, etc.)
  - `feat: <what was added>` for added features (for pull request titles, this takes precedence over `fix` and `chore`)
    - e.g. `feat: add server selection interface`
  - `fix: <what is fixed>` for bug and test fixes (for pull request titles, this takes precedence over `chore`)
    - e.g. `fix: added null-check to prevent crash`, `fix: removed outdated expectation in TestTryAddFriend`
  - `chore: <what was done>` for documentation, additional tests, non-functional code quality improvements
    - e.g. `chore: documented configuration options`, `chore: added tests for MathHelper`, `chore: resolved null reference warnings`

### Commit Quality

Commit messages should be concise and meaningful, but using additional lines in the commit message is valid and appropriate.

Commits should be coherent chunks of code, and ideally can compile.

Do:
- commit test code separately and after the feature code that it covers (within the same pull request)
- add documentation for new code in the same commit it was introduced
- commit immediately after all renames, file creation/deletion
  - `chore: renamed ClassA to ClassB`
  - `chore: separated class declaration into separate file`
- commit immediately after running automating tools that alter or generate code, making sure it compiles first
  - `chore: formatting`
-

Do not:
- commit a bug fix for already committed code in the same commit as new feature code
  - commit the bug fix, and then commit the new feature code
- commit multiple separate bug fixes in one commit
- commit code that has been modified by hand and modified by an automated tool in one commit

### Pull Request Quality

#### One Pull Request, One Issue

For organizational purposes, pull requests should all be associated to _one_ issue, and adddress _one_ problem.

Exceptions that do not require associated issues:
- Pull requests to add yourself to [`AUTHORS.md`](AUTHORS.md) if we miss it on your first commit
  - This type of pull request should be opened against the most "official" branch that any of your commits in (for example if you have commits in `main`, you should be listed in the [`AUTHORS.md`](AUTHORS.md) for that branch. If you have none in that branch (yet), but have commits in `prerelease`, then the pull request should be opened against that branch.
- Pull requests to for propagating changes between release branches (only done by staff)
- Code quality pull requests
  - Adding documentation
  - Adding tests
  - _Minor_ resolutions compiler warnings/messages (any resolutions that change type/method/property/field signatures or return values should have an associated issue)

Exceptions to the "one issue"/"one problem" rule-of-thumb:
- If the pull request is a refactor of an area of the code base, it may be the case that a proper refactor also resolves multiple issues, and in _this case_ associating multiple issues/problems to the pull request makes sense. In the case of a large refactor, a checklist of resolved problems should be provided in addition to the "Resolves" section of the pull request.

#### Associating Issues

Pull requests can _**and should**_ be associated to issues by starting the pull request text with `Resolves #101, #102`.

#### Validation/Verification

Pull request authors should provide screenshots, recordings, and/or other useful supplementary files or pieces of data to demonstrate the changes

If the pull request is
- resolving a bug
  - and the bug includes supplementary materials, please include them to show the "before" state
  - and the bug does not include supplementary materials, please take screenshots/record the before state/provide crash logs
- adding functionality no "before" supplementary materials are necessary (in theory they would not exist)
- resolving a crash log no "after" supplementary materials are necessary
- changing something that is visible in the interface of any component application screenshots/recordings should be provided
- changing something that is on the command line, for example adding command line arguments, examples of the input and the effects should be provided

#### Merge Conflicts

Pull request authors are responsible for resolving any merge conflicts with their pull requests. Please do not expect anyone to do this for you.

After resolving merge conflicts, please perform all of the testing done to produce any supplementary materials, and any changes should be documented with additional supplementary materials. This is to reduce unintended side effects of multiple pull requests interacting with each other.

#### Assets

All assets should be submitting in the [AscensionGameDev/Intersect-Assets](https://github.com/AscensionGameDev/Intersect-Assets) repository, in the `_upgrade` branch that matches the target branch for the pull request to this repository.

#### Your First Merged Pull Request

When your first pull request is ready to be merged or if it is a [simple enough change](https://github.com/AscensionGameDev/Intersect-Engine/pull/1102) please make sure you are listed in [`AUTHORS.md` under "Intersect Contributors"](AUTHORS.md#intersect-contributors).

#### Pull Request Maintenance

Please regularly rebase your pull request to keep it up to date with changes in the target branch, making sure to resolve merge conflicts, re-test and fix bugs, and update supplementary materials along the way. This helps speed up the review and merging process.

### Merging Guidelines

There are two different types of pull requests in this repository, promotion and addition/subtraction and each have different merging guidelines.

#### General Merging Guidelines

1. Pull request _authors_ have ownership of resolving merge conflicts (but in certain cases we may want to resolve the conflicts ourselves, and we should be willing to provide _limited_ and _basic_ assistance in the resolution of merge conflicts; this is at the discretion of the person who would be providing help).
2. Pull requests should be demonstrated to work by the author of the commit (if multiple people are contributing to the pull request, the author of the most recent "functional" commit has the responsibility to test that their commit compiles and functions correctly).
3. Pull requests should not be merged until all checks are passing.
4. Pull requests ideally should not be merged unless the commits are signed, and signed commits are required against `main`.
5. Pull requests should be checked to make sure that
   - features are only being merged into `development`
   - code documentation should only be merged into `development`
   - markdown documentation can be merged into any relevant branch
   - `AUTHORS.md` should only be modified in branches where the pull request author has other changes already applied
   - test _additions_ can be merged against `prerelease` or `development`
   - test _fixes_ can be merged against any relevant branch
   - bug fixes can be merged against any branch
6. Pull requests should be reviewed by at least 1 contributor and at least 1 maintainer (only maintainers should merge)
   - repository owners have the discretion to merge documentation changes without review
   - repository owners have the discretion to merge bug fixes and test fixes with 1 reviewer
   - security fixes should be tested and reviewed in private, these pull requests should be opened and merged immediately after checks pass
   - all feature changes should have at least 2 reviewers
7. `LICENSE.md` files **should not be changed by anyone including repository owners** for any reason, _except in the following cases_
   - **all** people who have any commits in the code covered by the changing license **must consent on the pull request**
     - if any contributors are unreachable beyond reasonable good-faith effort license changes must be unanimous
     - if all contributors are reachable but there are dissenters
       - the remaining contributors must agree unanimously to revert (and replace) any contributions when changing the license
       - the revert-and-replace vote must be unanimous
     - automated/automatable contributions (formatting, renames, etc.) or single-solution changes are exempt from the unanimous consent requirements
   - new `LICENSE.md` files can be introduced _for new subdirectories_, [`LICENSE.md`](LICENSE.md) can be updated to reflect the additions
   - old `LICENSE.md` files can be removed _for completely deleted subdirectories_, [`LICENSE.md`](LICENSE.md) can be updated to reflect the removals

#### "Promotion" Pull Requests

These are pull requests that move code between the different release branches, usually promoting a bug fix from `main` to `prerelease`, or `prerelease` to `development`.

- Merges _**should not squash**_ any commits
- Merges should rebase the changes onto the `HEAD` of the target branch
   - If `prerelease` starts at `main`'s `a` commit, then `main`s `b` (and later) commits should be rebased _on top of_ `prerelease`'s `HEAD` commit (if the commits are no longer relevant, for example in code that was deleted, they can be omitted)
   - It is strongly recommended to use interactive rebasing (see the GitLens extension for Visual Studio Code for an example of graphical interactive rebases)

#### "Additive"/"Subtractive" Pull Requests

These are pull requests that add or delete code from the repository (_not_ moving code between branches).

- An example of an additive pull request is a pull request to fix a bug or introduce a new feature.

- An example of a subtractive pull request is a pull request to revert a prior commit.

- Merges _**should always squash**_ all commits (however maintainers may have pull requests where squashing is not desired, but this is a case-by-case exception).

### Additional Resources
- [Creating good pull requests](http://seesparkbox.com/foundry/creating_good_pull_requests)
- [How to write the perfect pull request](https://github.com/blog/1943-how-to-write-the-perfect-pull-request)


