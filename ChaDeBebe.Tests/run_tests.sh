# Executa todos os testes da solução
dotnet test

# Executa apenas os testes que falharam na última vez (útil para debug)
dotnet test --failed

# Executa e mostra o log detalhado de cada [Fact]
dotnet test --logger "console;verbosity=detailed"