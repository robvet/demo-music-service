# demo-music-service
Simple API Service with Sql Database demo app

Use for quick demo to deploy API app with database and observability.

Deploy .NET Core Binaries or as a Container.

Need to provide following env vars:

 "catalogdbsecret": "<connection string for SqlDB>",
  "aiinstrumkeysecret": "<Instrumentation key for Application Insights>",
  "Data": { "UseInMemoryStore": "false" },    // Instructs Entity Framework whether to use In-Memory database
