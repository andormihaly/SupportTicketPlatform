# Aspire Testing and Deployment

## Testing

Aspire integration testing can start an AppHost and exercise multiple resources together. Use it when the behavior being tested genuinely depends on distributed orchestration or resource integration.

Do not replace focused unit tests with full distributed tests.

## Deployment

Aspire can contribute deployment manifests or target integrations depending on the installed version and target platform.

Before deployment:

- verify the CLI/integration version and whether the feature is stable or preview;
- identify the actual target platform;
- review generated manifests and infrastructure;
- keep secrets in secure external configuration;
- validate health, networking, identity, and observability on the target platform.

Local AppHost behavior is not proof that the production architecture is correct.
