# Especificações do Projeto

Para a especificação do projeto, foram determinadas as personas e suas histórias de usuários. Além disso, foram especificados os requisitos funcionais, não funcionais, restrições do projeto e o diagrama de casos de uso.

## Personas
<table style="border-collapse: collapse;">
  <tr>
    <td style="border: 1px solid black; padding: 10px;">
      <img src="https://i.imgur.com/neNHn6v.png"" alt="Ana Clara Lima" style="max-width: 100%; height: auto;">
    </td>
    <td style="border: 1px solid black; padding: 10px;">
      <img src="https://i.imgur.com/f3CJqC6.png" alt="Luiz da Silva" style="max-width: 100%; height: auto;">
    </td>
  </tr>
</table>

## Histórias de Usuários

Com base na análise das personas forma identificadas as seguintes histórias de usuários:

|EU COMO... `PERSONA`| QUERO/PRECISO ... `FUNCIONALIDADE` |PARA ... `MOTIVO/VALOR`                 |
|--------------------|------------------------------------|----------------------------------------|
|Usuário do sistema  | Registrar minhas despesas e ter uma previsão de quando poderei fazer minhas viagens | Ter um futuro confortável e conquistar liberdade geográfica |
|Usuário do sistema  | Registrar minhas metas financeiras | Ser independente financeiramente |
|Usuário do sistema  | Gerenciar minhas principais categorias de gastos | Ter estabilidade financeira |
|Usuário do sistema  | Acompanhar o fluxo entre receitas e despesas, além de verificar meus gastos em uma data específica | Obter reeducação financeira e conquistar uma casa própria |

## Requisitos

As tabelas que se seguem apresentam os requisitos funcionais e não funcionais, além das restrições que detalham o escopo do projeto.

### Requisitos Funcionais

|ID    | Descrição do Requisito  | Prioridade |
|------|-----------------------------------------|----|
|RF-001| Permitir que o usuário gerencie sua conta. | ALTA |
|RF-002| Permitir que o usuário gerencie carteiras. | ALTA |
|RF-003| Permitir que o usuário gerencie transações de entrada e saída. | ALTA |
|RF-004| Permitir que o usuário gerencie categorias para transações. | ALTA |
|RF-005| Permitir que o usuário gerencie metas financeiras pessoais | MÉDIA |
|RF-006| Permitir que o usuário visualize o saldo de entradas, saídas e o total. | ALTA |
|RF-007| Permitir que o usuário filtre suas transações por data, tipo e categoria. | MÉDIA |

### Requisitos não Funcionais

|ID     | Descrição do Requisito | Prioridade |
|-------|-------------------------|----|
|RNF-001| O site deve ser publicado em um ambiente acessível publicamente na Internet (Repl.it, GitHub Pages, Heroku, Vercel ou Microsoft Azure). | ALTA | 
|RNF-002| O site deve ser compatível e capaz de rodar nos principais navegadores do mercado (Google Chrome, Firefox, Microsoft Edge) com alterações mínimas. |  ALTA | 
|RNF-003| O site deve cumprir com a Lei Geral de Proteção de Dados (LGPD). | ALTA | 
|RNF-004| O site deve ser responsivo e deve haver otimização de imagens. | ALTA | 
|RNF-005| O site deve atender aos principais critérios de acessibilidade visual. | MÉDIA |

## Restrições

O projeto está restrito pelos itens apresentados na tabela a seguir.

|ID| Restrição                                             |
|--|-------------------------------------------------------|
|RE-001| O projeto deverá ser entregue no final do semestre letivo. |
|RE-002| Não é permitido à equipe terceirizar o desenvolvimento do trabalho. |
|RE-003| Deve-se utilizar uma navegação clara, com as tags html apropriadas, para que os usuários que usam leitores de tela possam navegar com mais precisão, a fim de cumprir o que foi definido na RNF-005. |
|RE-004| O design do site deve seguir as diretrizes e requisitos estabelecidos, como o uso de cores específicas, logotipos e estilo de fonte. |

## Diagrama de Casos de Uso

<img src="https://i.imgur.com/J2sxDQO.png" style="width: 100%;" alt="Diagrama de casos de uso">
