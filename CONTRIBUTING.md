# How to Contribute

We welcome third-party patches - they're essential to support the huge number of devices and platforms OccupOS is capable of running on. OccupOS was designed from the ground up to be extensible, so making changes should be a breeze. There are however a few guidelines to keep the project running smoothly.

## Prerequisites 

Before you do anything, make sure you have a [GitHub account](https://github.com/signup/free). Depending on your proposed contribution, the submission process varies; please refer to the table below when making your first contribution.

Step | Bug Fixes | Backlog Features | New Features
--- | --- | --- | ---
1 | Submit a new issue (assuming one does not exist) | - | Submit a new issue (assuming one does not exist) with the label 'type:feature suggestion'
2 | Fork the repository on GitHub | Fork the repository on GitHub | Wait until a maintainer comments on the suggestion and follow their advice. If they move the suggested feature to the backlog, proceed to the next step
3 | - | - | Fork the repository on GitHub

## General

* Ensure your patch matches our documented [code style guidelines](http://github.com/OccupOS/OccupOS/wiki/Code-Style-Guidelines).
* Try to keep patches small and focused (e.g. support for a single new sensor or a single bug fix).
* Patches with unnecessary formatting changes will be rejected (e.g. the addition of trailing whitespace and/or newlines).

## Refactoring

We do not encourage major refactors unless they are focused on performance or reliability. The majority of the existing codebase has been fully tested in multiple environments and though we like professional looking code, we cannot justify risking the introduction of bugs for cosmetic reasons only. Having said that, small sections of readability refactoring will be reviewed and merged in if deemed acceptable.

## Making Changes

* Create a branch from where you want to base your work.
  * This is usually the 'master' branch.
  * Only target release branches if you are certain your fix must be on that
    branch.
* Make commits of logical units.
* Check for unnecessary whitespace with `git diff --check` before committing.
* Make sure your commit messages [reference the issue](https://github.com/blog/831-issues-2-0-the-next-generation).
* Make sure you have added any necessary tests for your changes.

## Submitting Changes

* Ensure that your patch meets our [contribution guidelines](.
* Push your changes to a branch in your fork of the repository.
* Submit a pull request to the repository in the OccupOS organization.
* Watch the OccupOS build server to ensure that your patch builds correctly and passes all tests.

# Additional Resources

* [A great beginners guide to pull requests with GitHub](http://www.openshift.com/wiki/github-workflow-for-submitting-pull-requests)
