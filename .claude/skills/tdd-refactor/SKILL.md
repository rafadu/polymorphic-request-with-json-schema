---
name: tdd-refactor
description: >
  Use this skill whenever the user wants to refactor existing code while keeping tests green,
  says "todos os testes estão verdes", "refatore mantendo os testes passando", "refactor phase",
  "clean up the code", "não adicione comportamento", "melhore o design sem mudar o comportamento",
  or any TDD refactor-phase language. Always use this skill when tests are passing and the goal
  is structural improvement only — never add or change behavior.
---

# TDD Refactor Phase Skill

Melhora o design do código de produção **sem alterar comportamento**. Os testes são o contrato — se quebrarem, o refactor foi inválido.

---

## Step 1: Read CLAUDE.md

Leia `CLAUDE.md` no root do projeto. Ele define:

- **Linguagem / framework**
- **Comando de teste** para rodar a suíte completa
- **Guias de estilo ou linting** (ex: ESLint, Pylint, Checkstyle)
- **Padrões arquiteturais do projeto** (ex: Clean Architecture, DDD, Hexagonal)

---

## Step 2: Confirme o estado verde antes de começar

Rode o comando de teste **antes de qualquer mudança**.

- ✅ Todos passando → prossiga
- ❌ Algum falhando → **pare**. Informe o usuário: "Os testes precisam estar todos verdes antes de refatorar." Não continue.

---

## Step 3: Analise o código-alvo

Leia o arquivo indicado pelo usuário. Identifique os problemas de design presentes:

| Smell | Exemplos |
|---|---|
| **Duplicação** | Lógica repetida em múltiplos métodos |
| **Método longo** | Método com mais responsabilidades do que deveria |
| **Classe grande** | Classe com múltiplas razões para mudar |
| **Nome obscuro** | Variável/método cujo nome não revela intenção |
| **Magic values** | Números ou strings literais sem nomeação |
| **Condicionais complexas** | `if` aninhados, flags booleanas como parâmetros |
| **Acoplamento desnecessário** | Dependência direta onde uma abstração caberia |
| **Código morto** | Métodos, imports ou variáveis não utilizados |

Liste os smells encontrados antes de escrever qualquer código.

---

## Step 4: Planeje as transformações

Para cada smell, mapeie a refatoração correspondente:

| Smell | Refatoração |
|---|---|
| Duplicação | Extract Method / Extract Function |
| Método longo | Extract Method, decomposição em steps |
| Classe grande | Extract Class, separação de responsabilidades |
| Nome obscuro | Rename Variable / Rename Method |
| Magic values | Extract Constant / Extract Enum |
| Condicionais complexas | Extract Method, Replace Conditional with Polymorphism |
| Acoplamento | Introduce Interface / Inject Dependency |
| Código morto | Delete |

**Ordem importa:** faça uma transformação por vez. Não agrupe múltiplos refactors em uma única edição.

---

## Step 5: Aplique as transformações incrementalmente

Para **cada transformação**:

1. Faça a mudança no código
2. Rode o comando de teste
3. ✅ Verde → continue para a próxima transformação
4. ❌ Vermelho → desfaça a última mudança, analise, corrija antes de prosseguir

### Regras (inegociáveis)

- ✅ Uma transformação por vez
- ✅ Roda os testes após cada transformação
- ✅ O comportamento externo deve ser idêntico antes e depois
- ❌ Não adicione métodos, parâmetros ou classes que os testes não cobrem
- ❌ Não mude assinaturas públicas sem verificar todos os pontos de uso
- ❌ Não misture refactor com correção de bug ou nova feature
- ❌ Não "aproveite" para implementar algo que parece óbvio

---

## Step 6: Confirme verde ao final

Após todas as transformações, rode a suíte completa uma última vez.

Mostre o output. Declare: "✅ Todos os testes passando. Refactor concluído."

---

## Step 7: Output final

Entregue:
1. Lista dos smells identificados
2. Lista das transformações aplicadas (uma por linha)
3. O(s) arquivo(s) refatorado(s) — conteúdo final completo
4. Output do último run dos testes confirmando verde

---

## Edge cases

| Situação | Ação |
|---|---|
| Teste quebra durante o refactor | Desfaça a última mudança, analise a causa, ajuste a transformação |
| Usuário pede para adicionar feature "já que está aqui" | Recuse: "Isso é uma nova feature — finalize o refactor primeiro, depois abrimos um novo ciclo TDD" |
| Código sem cobertura de teste suficiente | Avise o usuário: "Esta parte não tem cobertura — refatorar aqui é arriscado. Sugiro escrever o teste primeiro." |
| Múltiplos arquivos afetados por uma extração | Atualize todos os pontos de uso, rode os testes após cada arquivo |
| Renomeação de símbolo público (API/interface) | Avise o usuário sobre breaking change antes de aplicar |