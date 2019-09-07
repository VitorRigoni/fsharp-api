FROM mcr.microsoft.com/mssql/server:2017-CU8-ubuntu as adventureworks

RUN mkdir /bak
ADD https://github.com/Microsoft/sql-server-samples/releases/download/adventureworks/AdventureWorks2017.bak /bak/AdventureWorks2017.bak

CMD /opt/mssql/bin/sqlservr & sleep 90s && /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U SA -P "yourStrong(!)Password" \
   -Q "RESTORE DATABASE [AdventureWorks] FROM DISK = N'/bak/AdventureWorks2017.bak' WITH FILE = 1, MOVE N'AdventureWorks2017' TO N'/var/opt/mssql/data/AdventureWorks.mdf', MOVE N'AdventureWorks2017_log' TO N'/var/opt/mssql/data/AdventureWorks_log.ldf', NOUNLOAD, STATS = 10"
