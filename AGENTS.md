# C-Sweet Work Management Contracts contributor instructions

## Package versioning

- Every edit to the contracts project must increment the package version in
  `src/CSweet.WorkManagement.Contracts/CSweet.WorkManagement.Contracts.csproj` before the work is
  complete. Local project references can hide a stale package version, so an unchanged version is
  not acceptable.
- Use semantic versioning: patch/build for compatible maintenance, minor for additive public
  contracts, and major for breaking contracts, unless the user requests a specific version.
- Update pinned `CSweet.WorkManagement.Contracts` references in downstream repositories changed as
  part of the same work.
- Run `dotnet test CSweet.WorkManagement.Contracts.slnx` and pack the contracts project. Confirm the
  generated `.nupkg` filename and NuGet metadata contain the new version before handoff.
