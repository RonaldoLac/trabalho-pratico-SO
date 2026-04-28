Markdown# Algoritmo do Banqueiro (Multithreaded) - C#

Este repositório contém a implementação do Algoritmo do Banqueiro, um método clássico de evasão de deadlocks em sistemas operacionais. O projeto foi desenvolvido como requisito do trabalho prático realizado pelos alunos Kauan Gomes Marques e Ronaldo Augusto Oliveira Lacerda da disciplina de Sistemas Operacionais.

A aplicação simula o comportamento de múltiplos clientes (threads) que solicitam e liberam recursos concorrentemente. A aprovação das requisições é gerida por um algoritmo que verifica continuamente se a alocação de recursos manterá o sistema em um estado seguro, negando solicitações que possam levar a impasses estruturais (*deadlocks*).

## Tecnologias Utilizadas
* **Linguagem:** C# (.NET)
* **Concorrência:** Utilização da classe `System.Threading.Thread` para simulação de múltiplos clientes.
* **Sincronização:** Controle de exclusão mútua em variáveis compartilhadas por meio de blocos `lock` (equivalente ao *mutex*).

## Instruções de Compilação e Execução

1. Clone este repositório ou realize o download dos arquivos fonte.
2. Acesse o diretório raiz do projeto por meio do terminal de comando.
3. A execução exige a definição prévia da quantidade de recursos disponíveis, que deve ser passada como argumento via linha de comando.

**Sintaxe de execução:**
```bash
dotnet run <recurso_1> <recurso_2> <recurso_3>
Exemplo de uso:Bashdotnet run 10 5 7
Neste cenário, o sistema é inicializado com 10 instâncias do primeiro tipo de recurso, 5 do segundo e 7 do terceiro.Estruturas de DadosO controle lógico dos recursos é realizado por meio das matrizes e vetores descritos abaixo, atualizados a cada iteração de solicitação e liberação:EstruturaDescriçãoavailableQuantidade de recursos disponíveis de cada tipo no sistema.maximumDemanda máxima de cada cliente por tipo de recurso.allocationQuantidade de recursos correntemente alocados a cada cliente.needNecessidade remanescente do cliente (maximum - allocation).Funções PrincipaisIsSafe(): Verifica se a concessão dos recursos mantém o sistema em um estado seguro, simulando temporariamente o término dos processos antes de efetivar a alocação.RequestResources(): Processa a solicitação de recursos de um cliente de forma atômica. Retorna 0 em caso de sucesso ou -1 caso o pedido seja negado para prevenção do deadlock.ReleaseResources(): Libera os recursos retidos por um cliente, adicionando-os novamente à estrutura available.Estrutura de DiretóriosPlaintextatividade POO/
├── Program.cs         # Código-fonte principal da aplicação
├── relatorio.html     # Relatório formatado (versão para exportação em PDF)
├── relatorio.md       # Relatório textual em formato Markdown
├── readme.md          # Documentação principal do repositório
├── atividade POO.csproj
└── atividade POO.sln

Relatório: A documentação completa, contendo a fundamentação teórica, a metodologia adotada e a análise dos resultados de execução, encontra-se disponível nos arquivos abaixo:relatorio.html - Recomendado para leitura e exportação em formato PDF.relatorio.md - Código-fonte do relatório.

Link do Repositório Público: https://github.com/RonaldoLac/trabalho-pratico-SO