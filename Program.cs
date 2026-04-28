using System;
using System.Threading;


/// Implementação do Algoritmo de Banqueiro (Banker's Algorithm) em C#.
/// Este algoritmo previne deadlock ao verificar a segurança do estado antes de alocar recursos.
/// Múltiplas threads (clientes) competem por recursos limitados de forma segura.

class Program
{
    const int NumberOfCustomers = 5;    // Número de threads/clientes concorrentes
    const int NumberOfResources = 3;    // Número de tipos de recursos disponíveis

    // Estruturas de dados do Algoritmo de Banqueiro:
    // - available[j]: Quantidade de recurso j disponível no sistema
    // - maximum[i,j]: Demanda máxima do cliente i pelo recurso j
    // - allocation[i,j]: Quantidade de recurso j atualmente alocada ao cliente i
    // - need[i,j]: Quantidade ainda necessária do recurso j pelo cliente i (maximum - allocation)
    
    static int[] available = new int[NumberOfResources];
    static int[,] maximum = new int[NumberOfCustomers, NumberOfResources];
    static int[,] allocation = new int[NumberOfCustomers, NumberOfResources];
    static int[,] need = new int[NumberOfCustomers, NumberOfResources];

    // Sincronização: Garante acesso exclusivo às estruturas de dados do banqueiro,
    // evitando condições de corrida quando múltiplas threads acessam simultaneamente
    static readonly object _lock = new object();

    static void Main(string[] args)
    {
        // Valida se exatamente 3 valores foram passados (um para cada tipo de recurso)
        if (args.Length != NumberOfResources)
        {
            Console.WriteLine("Uso: dotnet run <rec1> <rec2> <rec3>");
            return;
        }

        // Carrega os valores de recursos disponíveis do parâmetro de linha de comando
        // e valida se são números inteiros válidos
        for (int i = 0; i < NumberOfResources; i++)
        {
            if (!int.TryParse(args[i], out available[i]))
            {
                Console.WriteLine($"Erro: O valor '{args[i]}' não é um número válido.");
                return;
            }
        }

        // Define aleatoriamente a demanda máxima de cada cliente por cada recurso
        // allocation inicia com zero (nenhum recurso foi alocado ainda)
        // need inicia igual a maximum (toda a demanda está pendente)
        for (int i = 0; i < NumberOfCustomers; i++)
        {
            for (int j = 0; j < NumberOfResources; j++)
            {
                maximum[i, j] = Random.Shared.Next(1, available[j] + 1);
                allocation[i, j] = 0;
                need[i, j] = maximum[i, j];
            }
        }

        // Cria uma thread para cada cliente que solicitará e liberará recursos
        Thread[] threads = new Thread[NumberOfCustomers];
        for (int i = 0; i < NumberOfCustomers; i++)
        {
            int customerNum = i; // Captura por valor para evitar conflito de closure
            threads[i] = new Thread(() => CustomerThread(customerNum));
            threads[i].Start();
        }

        // Aguarda o término de todas as threads (neste caso, ficarão em loop infinito)
        foreach (var t in threads)
        {
            t.Join();
        }
    }


    /// Algoritmo de Segurança: Verifica se o sistema está em estado seguro.
    /// Um estado é seguro se existe uma sequência de alocação que permite todos os clientes
    /// terminarem sem causar deadlock. Simula a conclusão dos processos em sequência.

    static bool IsSafe()
    {
        // Cria uma cópia de 'available' para simular as alocações sem modificar o estado real
        int[] work = new int[NumberOfResources];
        Array.Copy(available, work, NumberOfResources);
        
        // Rastreia quais clientes já terminaram sua execução na simulação
        bool[] finish = new bool[NumberOfCustomers];

        // Tenta encontrar uma sequência segura que permita todos os clientes terminarem
        int count = 0;
        while (count < NumberOfCustomers)
        {
            bool found = false;
            
            // Procura um cliente que ainda não terminou e pode ser satisfeito com os recursos disponíveis
            for (int i = 0; i < NumberOfCustomers; i++)
            {
                if (!finish[i])
                {
                    // Verifica se o cliente i pode ter toda sua necessidade satisfeita com 'work'
                    int j;
                    for (j = 0; j < NumberOfResources; j++)
                    {
                        if (need[i, j] > work[j]) break; // Recurso insuficiente
                    }

                    // Se todos os recursos são suficientes (j alcançou NumberOfResources)
                    if (j == NumberOfResources)
                    {
                        // Simula a conclusão do cliente: retorna seus recursos alocados
                        for (int k = 0; k < NumberOfResources; k++)
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
        
        // Se todos os clientes puderam terminar, o estado é seguro
        return true;
    }

    /// Processa uma solicitação de recursos do cliente, garantindo atomicidade e segurança.
    /// Retorna 0 se aprovado, -1 se negado.

    static int RequestResources(int customerNum, int[] request)
    {
        // Lock garante que apenas uma thread por vez acessa o estado crítico,
        // prevenindo race conditions nas estruturas de dados compartilhadas
        lock (_lock)
        {
            // Validação 1: A solicitação não pode exceder a necessidade declarada pelo cliente
            // Isso previne abusos onde um cliente pede mais que sua demanda máxima
            for (int i = 0; i < NumberOfResources; i++)
            {
                if (request[i] > need[customerNum, i]) return -1;
            }

            // Validação 2: Verifica se há recursos suficientes disponíveis no sistema
            for (int i = 0; i < NumberOfResources; i++)
            {
                if (request[i] > available[i]) return -1;
            }

            // Fase de alocação tentativa: Modifica o estado como se a alocação fosse aceita
            for (int i = 0; i < NumberOfResources; i++)
            {
                available[i] -= request[i];           // Remove dos recursos disponíveis
                allocation[customerNum, i] += request[i]; // Adiciona ao cliente
                need[customerNum, i] -= request[i];   // Reduz a necessidade restante
            }

            // Verifica se a alocação mantém o sistema em estado seguro
            if (IsSafe())
            {
                Console.WriteLine($"Cliente {customerNum}: Pedido APROVADO.");
                return 0; // Alocação aceita permanentemente
            }
            else
            {
                // Desfaz a alocação (rollback) pois levaria a um estado inseguro/deadlock
                for (int i = 0; i < NumberOfResources; i++)
                {
                    available[i] += request[i];
                    allocation[customerNum, i] -= request[i];
                    need[customerNum, i] += request[i];
                }
                Console.WriteLine($"Cliente {customerNum}: Pedido NEGADO (Prevenção de Deadlock).");
                return -1; // Alocação rejeitada
            }
        }
    }


    /// Libera recursos previamente alocados, devolvendo-os ao pool disponível
    /// e aumentando a necessidade do cliente (ele pode pedir novamente).

    static int ReleaseResources(int customerNum, int[] release)
    {
        lock (_lock)
        {
            // Reduz o que foi alocado ao cliente e aumenta os recursos disponíveis
            for (int i = 0; i < NumberOfResources; i++)
            {
                allocation[customerNum, i] -= release[i];
                available[i] += release[i];
                need[customerNum, i] += release[i]; // A necessidade aumenta pois ele pode pedir novamente
            }
            Console.WriteLine($"Cliente {customerNum}: Recursos LIBERADOS.");
            return 0;
        }
    }


    /// Função executada por cada thread de cliente.
    /// Simula um cliente que periodicamente solicita, usa e libera recursos.
    /// Executa em loop infinito para demonstrar o comportamento do algoritmo.

    static void CustomerThread(int customerNum)
    {
        int[] request = new int[NumberOfResources];
        int[] release = new int[NumberOfResources];

        while (true)
        {
            // Aguarda um tempo aleatório antes de fazer uma nova solicitação
            Thread.Sleep(Random.Shared.Next(0, 3000));

            // Gera uma solicitação aleatória dentro da necessidade restante do cliente
            for (int i = 0; i < NumberOfResources; i++)
            {
                if (need[customerNum, i] > 0)
                    request[i] = Random.Shared.Next(0, need[customerNum, i] + 1);
                else
                    request[i] = 0;
            }

            // Tenta obter os recursos (aprovado ou negado pelo banqueiro)
            RequestResources(customerNum, request);
            
            // Simula o tempo de uso dos recursos alocados
            Thread.Sleep(Random.Shared.Next(0, 2000));

            // Libera parte aleatória dos recursos alocados para outros clientes usarem
            for (int i = 0; i < NumberOfResources; i++)
            {
                if (allocation[customerNum, i] > 0)
                    release[i] = Random.Shared.Next(0, allocation[customerNum, i] + 1);
                else
                    release[i] = 0;
            }
            
            ReleaseResources(customerNum, release);
        }
    }
}