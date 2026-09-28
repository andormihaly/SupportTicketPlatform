START TRANSACTION;

CREATE TABLE "TicketAnalyses" (
    "Id" uuid NOT NULL,
    "TicketId" uuid NOT NULL,
    "Content" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    CONSTRAINT "PK_TicketAnalyses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TicketAnalyses_Tickets_TicketId" FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_TicketAnalyses_TicketId" ON "TicketAnalyses" ("TicketId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260923120000_AddTicketAnalysis', '10.0.11');

COMMIT;
