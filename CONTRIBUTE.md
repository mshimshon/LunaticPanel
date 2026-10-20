## Commit Policy
You must use prefix for the commits `Fix:`, `Break:`, `Feat:`, `Note:` 
this is critical you never commit using those prefixes in anyway other than its intended use.
### Purpose?
To help us avoid having to keep track of versions instead CI/CD will figure out the next version on its own base on the commit severities.
But also it allows the CD to build the release note for each version with the specific information related to the list of commit contained in that release.

### Commit a feature:
when we add or finalize a feature we commit like so
```
Feat: Added new Feature XYZ
```

### Commit a Breaking change:
any change in signature or behavior that will cause breaking change MUST be documented as such on commit
```
Break: Added argument to method Interface.LoginAsync any consumer must update their code.
```

### Commit a Fix:
any fixes of internal code MUST be documented as such on commit
```
Fix: Cleaned Someone's Mess.
```

### Commit a Note or add along side:
any notes for the release note must be documented as such:
```
Note: The rule is simple, if it breaks, you blame Justin.
Note: Do not forget to star the repos, otherwise, i'll disable your panel remotely.
```

### Version Rules
`Break:` => 1.0.0 -> 2.0.0
`Feat:` => 2.0.0 -> 2.1.0
`Fix:` => 2.1.0 -> 2.1.1
`Note:` => Doesn't trigger CI/CD, any CI CD missing all three above will fail.

### Note
- Do not forget `:` the rule is `Prefix:` not `Prefix`.
- Commit Multiple line if needed.
- Not case sensistive `feat:` and `Feat:` are valid.
