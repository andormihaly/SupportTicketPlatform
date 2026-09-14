# Aspire CLI and Setup

Use the installed Aspire CLI version as the source of truth. Verify commands with `aspire --help` and `aspire --version` rather than assuming a command exists.

Typical tasks include `aspire new`, `aspire init`, `aspire run`, `aspire add`, `aspire config`, and `aspire update`.

Publishing/deployment commands can be version-dependent or preview features. Confirm availability before using them.

For an existing solution, `aspire init` can help add orchestration projects, but inspect generated changes before accepting them.

Keep AppHost separate from Domain, Application, and Infrastructure business projects.
