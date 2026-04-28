# Algoritmo do Banqueiro - Relatório Técnico

**Disciplina:** Sistemas Operacionais  
**Professor:** [Nome do Professor]  
**Data:** 28 de Abril de 2026  
**Aluno(s):** [Nome do Aluno]  

---

## 1. Introdução

O presente trabalho prático tem como objetivo a implementação do **Algoritmo do Banqueiro** (Banker's Algorithm), proposto por Edsger Dijkstra em 1965, como solução para o problema de deadlock em sistemas operacionais. Este algoritmo é uma das técnicas mais clássicas para prevenção de deadlocks, sendo amplamente estudado em disciplinas de Sistemas Operacionais.

### 1.1 Contextualização do Problema

Em sistemas computacionais, múltiplos processos frequentemente competem por recursos limitados (como memória, impressoras, arquivos, conexões de rede, etc.). Quando os processos solicitam recursos de forma inadequada, pode ocorrer uma situação de **deadlock** - um estado onde dois ou mais processos ficam bloqueados indefinidamente, esperando uns pelos outros para liberar os recursos que precisam.

O Algoritmo do Banqueiro é uma abordagem que visa prevenir deadlocks ao garantir que o sistema sempre permaneça em um **estado seguro**. Um estado é considerado seguro quando existe uma sequência de alocação de recursos que permite que todos os processos completem sua execução, mesmo em um cenário adverso.

### 1.2 Objetivos do Trabalho

- Implementar um sistema multithreaded que simule o comportamento do algoritmo do banqueiro
- Demonstrar a prevenção de deadlocks através da verificação de estados seguros
- Utilizar mecanismos de sincronização para evitar condições de corrida
- Compreender na prática os conceitos de concorrência, sincronização e prevenção de deadlocks

### 1.3 Estrutura do Relatório

Este relatório está organizado da seguinte forma: a Seção 2 apresenta o desenvolvimento, com a explicação das estruturas de dados, do algoritmo de segurança e da implementação das funções de request e release. A Seção 3 mostra os resultados obtidos através de testes. Por fim, a Seção 4 apresenta as conclusões e considerações finais.

---

## 2. Desenvolvimento

### 2.1 Visão Geral da Implementação

A implementação foi desenvolvida em **C#** utilizando o framework .NET. O programa simula um sistema bancário onde múltiplos clientes (representados por threads) solicitam e liberam recursos concorrentemente. O "banqueiro" (implementação do algoritmo) avalia cada solicitação e approve-a apenas se o sistema permanecer em um estado seguro.

### 2.2 Estruturas de Dados

O algoritmo utiliza as seguintes estruturas de dados, conforme especificado no enunciado do trabalho:

```c
// Número de clientes e tipos de recursos
const int NUMBER_OF_CUSTOMERS = 5;
const int NUMBER_OF_RESOURCES = 3;

// Recursos disponíveis no sistema
static int[] available = new int[NUMBER_OF_RESOURCES];

// Demanda máxima de cada cliente
static int[,] maximum = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];

// Recursos atualmente alocados a cada cliente
static int[,] allocation = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];

// Necessidade remanescente de cada cliente (maximum - allocation)
static int[,] need = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
```

**Descrição das estruturas:**

- **available**: Vetor que armazena a quantidade de recursos disponíveis de cada tipo no sistema
- **maximum**: Matriz que representa a demanda máxima de cada cliente para cada tipo de recurso
- **allocation**: Matriz que mostra os recursos atualmente alocados a cada cliente
- **need**: Matriz que representa a necessidade remanescente de cada cliente (maximum - allocation)

### 2.3 Algoritmo de Segurança

O algoritmo de segurança é o coração do Algoritmo do Banqueiro. Ele verifica se o sistema está em um estado seguro, ou seja, se existe uma sequência de alocação que permite que todos os clientes completem sua execução.

```c
static bool IsSafe()
{
    // Cria uma cópia de 'available' para simular as alocações
    int[] work = new int[NUMBER_OF_RESOURCES];
    Array.Copy(available, work, NUMBER_OF_RESOURCES);
    
    // Rastreia quais clientes já terminaram
    bool[] finish = new bool[NUMBER_OF_CUSTOMERS];

    // Tenta encontrar uma sequência segura
    int count = 0;
    while (count < NUMBER_OF_CUSTOMERS)
    {
        bool found = false;
        
        // Procura um cliente que pode ser satisfeito
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            if (!finish[i])
            {
                // Verifica se a necessidade do cliente pode ser satisfeita
                int j;
                for (j = 0; j < NUMBER_OF_RESOURCES; j++)
                {
                    if (need[i, j] > work[j]) break;
                }

                // Se todos os recursos são suficientes
                if (j == NUMBER_OF_RESOURCES)
                {
                    // Simula a conclusão do cliente
                    for (int k = 0; k < NUMBER_OF_RESOURCES; k++)
                    {
                        work[k] += allocation[i, k];
                    }
                    finish[i] = true;
                    found = true;
                    count++;
                }
            }
        }
        
        // Se nenhum cliente pode ser satisfeito, há risco de deadlock
        if (!found) return false;
    }
    
    return true;
}
```

**Funcionamento do algoritmo:**
1. Cria uma cópia dos recursos disponíveis (work)
2. Para cada cliente não terminado, verifica se sua necessidade pode ser satisfeita com os recursos disponíveis
3. Se possível, simula a conclusão do cliente (retorna seus recursos alocados ao pool)
4. Repete o processo até que todos os clientes possam terminar ou até detectar um estado inseguro

### 2.4 Função de Solicitação de Recursos (request_resources)

A função `RequestResources` é responsável por processar as solicitações de recursos dos clientes. Ela segue o seguinte fluxo:

```c
static int RequestResources(int customerNum, int[] request)
{
    lock (_lock)  // Garante acesso exclusivo às estruturas compartilhadas
    {
        // 1. Valida se a solicitação não excede a necessidade do cliente
        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
        {
            if (request[i] > need[customerNum, i]) return -1;
        }

        // 2. Verifica se há recursos suficientes disponíveis
        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
        {
            if (request[i] > available[i]) return -1;
        }

        // 3. Aloca tentativamente os recursos
        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
        {
            available[i] -= request[i];
            allocation[customerNum, i] += request[i];
            need[customerNum, i] -= request[i];
        }

        // 4. Verifica se o estado permanece seguro
        if (IsSafe())
        {
            return 0; // Aprovado
        }
        else
        {
            // Desfaz a alocação (rollback)
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                available[i] += request[i];
                allocation[customerNum, i] -= request[i];
                need[customerNum, i] += request[i];
            }
            return -1; // Negado
        }
    }
}
```

### 2.5 Função de Liberação de Recursos (release_resources)

A função `ReleaseResources` devolve os recursos previamente alocados ao pool disponível:

```c
static int ReleaseResources(int customerNum, int[] release)
{
    lock (_lock)
    {
        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
        {
            allocation[customerNum, i] -= release[i];
            available[i] += release[i];
            need[customerNum, i] += release[i];
        }
        return 0;
    }
}
```

### 2.6 Implementação das Threads de Clientes

Cada cliente é implementado como uma thread separada que executa um loop contínuo:

```c
static void CustomerThread(int customerNum)
{
    while (true)
    {
        // Espera aleatória antes de fazer uma solicitação
        Thread.Sleep(Random.Shared.Next(0, 3000));

        // Gera solicitação aleatória dentro da necessidade
        // ...
        
        RequestResources(customerNum, request);
        
        // Simula uso dos recursos
        Thread.Sleep(Random.Shared.Next(0, 2000));

        // Libera parte dos recursos
        // ...
        
        ReleaseResources(customerNum, release);
    }
}
```

### 2.7 Sincronização

Para prevenir **condições de corrida** (race conditions), foi utilizado o mecanismo de `lock` do C#, que é equivalente a um mutex. O lock garante que apenas uma thread por vez possa acessar as estruturas de dados compartilhadas do banqueiro, garantindo a atomicidade das operações.

### 2.8 Inicialização do Sistema

O programa é invoked com três parâmetros representando a quantidade inicial de recursos disponíveis:

```bash
dotnet run 10 5 7
```

Onde:
- 10 = quantidade do recurso tipo 1
- 5 = quantidade do recurso tipo 2
- 7 = quantidade do recurso tipo 3

A demanda máxima de cada cliente é gerada aleatoriamente, garantindo que cada cliente possa solicitar até a quantidade total de recursos disponíveis de cada tipo.

---

## 3. Resultados

### 3.1 Testes Realizados

A implementação foi testada com diferentes configurações de recursos iniciais. Os testes demonstraram que o algoritmo funciona corretamente, aprovando solicitações que mantêm o sistema em estado seguro e negando aquelas que levariam a um estado inseguro.

**Exemplo de execução com 10 5 7:**

```
Cliente 2: Pedido APROVADO.
Cliente 0: Pedido APROVADO.
Cliente 3: Pedido APROVADO.
Cliente 1: Pedido NEGADO (Prevenção de Deadlock).
Cliente 4: Pedido APROVADO.
Cliente 2: Recursos LIBERADOS.
Cliente 0: Recursos LIBERADOS.
...
```

### 3.2 Comportamento Observado

1. **Prevenção de Deadlock**: O algoritmo nunca aprova uma solicitação que leve a um estado inseguro, prevenindo efetivamente deadlocks.

2. **Concorrência**: Múltiplas threads acessam as estruturas de dados compartilhadas simultaneamente sem corromper os dados, graças ao mecanismo de lock.

3. **Justiça**: As solicitações são processadas de forma justa, com clientes tendo seus pedidos negados temporariamente quando a aprovação levaria a um estado inseguro.

### 3.3 Casos de Teste

| Recursos Iniciais | Comportamento Esperado | Resultado |
|------------------|----------------------|-----------|
| 10 5 7 | Sistema inicia com recursos suficientes | OK |
| 3 3 3 | Recursos limitados, mais negações | OK |
| 1 1 1 | Recursos mínimos, maioria negada | OK |
| 100 100 100 | Recursos abundantes, mais aprovações | OK |

---

## 4. Conclusão

Este trabalho prático permitiu a implementação prática do Algoritmo do Banqueiro, demonstrando de forma concreta os conceitos de concorrência, sincronização e prevenção de deadlocks em sistemas operacionais.

### 4.1 Contribuições do Trabalho

1. **Compreensão Teórica**: A implementação reforçou o entendimento teórico do algoritmo de segurança e sua importância na prevenção de deadlocks.

2. **Experiência com Multithreading**: O trabalho proporcionou experiência prática na criação e gerenciamento de múltiplas threads em C#.

3. **Sincronização**: A utilização de locks para controle de acesso a dados compartilhados demonstrou na prática como prevenir condições de corrida.

4. **Aplicação Prática**: O algoritmo foi aplicado em um cenário realista de sistema bancário com múltiplos clientes competindo por recursos.

### 4.2 Limitações e Trabalhos Futuros

- O programa executa em loop infinito; uma versão completa poderia incluir mecanismos de terminação
- A demanda máxima é gerada aleatoriamente; uma versão mais robusta poderia permitir configuração via arquivo
- Não há interface gráfica; uma versão futura poderia incluir visualização em tempo real do estado do sistema

### 4.3 Considerações Finais

O Algoritmo do Banqueiro permanece como uma das abordagens mais importantes para o problema de deadlock em sistemas operacionais. Embora sua aplicação prática seja limitada (requere conhecimento prévio das necessidades máximas dos processos), os conceitos envolvidos são fundamentais para a compreensão de técnicas mais modernas de gerenciamento de recursos.

---

## Referências

1. SILBERSCHATZ, Abraham; GALVIN, Peter B.; GAGNE, Greg. **Fundamentos de sistemas operacionais**. 9. ed. Rio de Janeiro, RJ: LTC, c2015. E-book. ISBN 978-85-216-3001-2.

2. DIJKSTRA, Edsger W. **Cooperating sequential processes**. In: Programming Languages. Springer, 1968. p. 65-138.

---

## Link do Repositório

[Repositório GitHub - Algoritmo do Banqueiro](https://github.com/RonaldoLac/trabalho-pratico-SO)

*O código fonte está disponível no repositório acima, acompanhado de instruções detalhadas de compilação e execução no arquivo README.md.*