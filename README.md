# Algoritmo do Banqueiro (Multithreaded) - C#

Este repositório contém a implementação do **Algoritmo do Banqueiro** (Banker's Algorithm), proposto por Edsger Dijkstra em 1965, desenvolvido como trabalho prático da disciplina de Sistemas Operacionais.

O objetivo do programa é simular um sistema bancário onde múltiplos clientes (threads) solicitam e liberam recursos concorrentemente. O banqueiro (algoritmo) avalia cada solicitação e só a aprova se o sistema permanecer em um **estado seguro**, prevenindo a ocorrência de *deadlocks*.

## 🛠️ Tecnologias Utilizadas
* **Linguagem:** C# (.NET)
* **Concorrência:** `System.Threading.Thread`
* **Sincronização:** Bloco `lock` (substituto nativo e seguro para *Mutexes*)

## ⚙️ Pré-requisitos
Para compilar e executar este projeto, você precisará ter o SDK do .NET instalado na sua máquina.
* [Download do .NET SDK](https://dotnet.microsoft.com/download)

## 🚀 Como Compilar e Executar

1. Clone este repositório ou baixe os arquivos fonte.
2. Abra o terminal na pasta raiz do projeto (onde está o arquivo `.csproj` ou `Program.cs`).
3. O programa exige que você passe a quantidade inicial de recursos disponíveis no "banco" diretamente via linha de comando.

A sintaxe de execução é:
```bash
dotnet run <recurso_1> <recurso_2> <recurso_3>
```

### Exemplo de execução:
```bash
dotnet run 10 5 7
```

Isso inicializa o sistema com:
- 10 unidades do recurso tipo 1
- 5 unidades do recurso tipo 2
- 7 unidades do recurso tipo 3

## 📊 Estrutura do Código

O código implementa as seguintes estruturas de dados conforme especificado no trabalho:

| Estrutura | Descrição |
|-----------|-----------|
| `available` | Quantidade de recursos disponíveis de cada tipo |
| `maximum` | Demanda máxima de cada cliente por cada recurso |
| `allocation` | Recursos atualmente alocados a cada cliente |
| `need` | Necessidade remanescente (maximum - allocation) |

### Funções Principais

- **`IsSafe()`**: Verifica se o sistema está em um estado seguro
- **`RequestResources()`**: Processa solicitações de recursos (retorna 0 se aprovado, -1 se negado)
- **`ReleaseResources()`**: Libera recursos previamente alocados

## 📁 Arquivos do Projeto

```
atividade POO/
├── Program.cs          # Código fonte principal
├── relatorio.html     # Relatório em formato HTML (para PDF)
├── relatorio.md       # Relatório em formato Markdown
├── redme.md           # Este arquivo
├── atividade POO.csproj
└── atividade POO.sln
```

## 📄 Relatório

O relatório completo está disponível em:
- **[relatorio.html](relatorio.html)** - Versão para impressão/PDF (abra no navegador e use "Salvar como PDF")
- **[relatorio.md](relatorio.md)** - Versão em Markdown

O relatório segue a estrutura exigida:
1. Introdução sobre o tema
2. Seção de Desenvolvimento
3. Seção de Resultados
4. Conclusão

## 🔗 Link do Repositório

Substitua pelo link do seu repositório público no GitHub:
```
https://github.com/RonaldoLac/trabalho-pratico-SO
```