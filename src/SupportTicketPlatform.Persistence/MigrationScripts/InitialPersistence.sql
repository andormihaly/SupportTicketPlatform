CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Tickets" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Description" character varying(4000) NOT NULL,
    "Status" character varying(50) NOT NULL,
    "OccurredAt" timestamp with time zone NOT NULL,
    "Embedding" vector(1536),
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    CONSTRAINT "PK_Tickets" PRIMARY KEY ("Id")
);

CREATE TABLE "Resolutions" (
    "Id" uuid NOT NULL,
    "TicketId" uuid NOT NULL,
    "Content" character varying(4000) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    CONSTRAINT "PK_Resolutions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Resolutions_Tickets_TicketId" FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Resolutions_TicketId" ON "Resolutions" ("TicketId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260921110153_InitialPersistence', '10.0.11');

COMMIT;

