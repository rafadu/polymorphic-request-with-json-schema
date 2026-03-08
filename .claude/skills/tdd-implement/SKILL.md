---
name: tdd-implement
description: >
  Use this skill whenever the user says the test is red/failing and wants the minimum production code to make it pass,
  mentions "green phase", "make the test pass", "o teste está vermelho", "mínimo de código para passar",
  "run the tests", "confirm it passes", or any TDD green-phase language. 
  Always use this skill when the user has a failing test and wants production code written — never write more than the minimum needed.
---

# TDD Green Phase Skill

Escreve o **mínimo de código de produção** para fazer o teste passar. Nada além disso.

---

## Step 1: Read CLAUDE.md

Leia `CLAUDE.md` no root do projeto. Ele define:

- **Linguagem / framework**
- **Test runner e comando de teste** (ex: `npm test`, `pytest`, `go test ./...`, `./gradlew test`)
- **Estrutura de pastas** (onde fica o código de produção vs testes)
- **Convenções de import/export**

Se `CLAUDE.md` estiver ausente ou incompleto, pergunte ao usuário antes de prosseguir.

---

## Step 2: Leia o teste

Leia o arquivo de teste que o usuário indicou (ou encontre-o na estrutura do projeto).

Identifique:
- O **módulo/classe/função** sendo testado
- O **path esperado** do código de produção (geralmente inferido do import no teste)
- O **comportamento exato** que o teste valida (assertion)
- O **tipo de falha atual**: arquivo não existe, função não existe, retorno errado, exceção não lançada

---

## Step 3: Escreva o mínimo de código de produção

### Regras (inegociáveis)

- ✅ Escreva **apenas** o suficiente para o teste passar
- ✅ O código deve estar no path exato que o teste importa
- ✅ Use a linguagem/estilo definido no `CLAUDE.md`
- ❌ Não antecipe casos de uso futuros
- ❌ Não adicione parâmetros, métodos ou classes extras
- ❌ Não refatore (isso é fase seguinte)
- ❌ Não escreva código "limpo" além do necessário para passar

### O que "mínimo" significa na prática

| Falha | Mínimo aceitável |
|---|---|
| Módulo não existe | Criar o arquivo com a função retornando o valor hardcoded |
| Função não existe | Adicionar a função ao módulo existente |
| Retorno errado | Corrigir só o retorno, sem refatorar o resto |
| Exceção não lançada | Adicionar só o `throw`/`raise` esperado |
| Efeito colateral ausente | Implementar só o efeito colateral testado |

**Hardcode é válido** se fizer o teste passar. Exemplo: se o teste espera `42`, retornar `42` diretamente é aceitável nesta fase.

---

## Step 4: Rode o comando de teste

Após escrever o arquivo de produção, execute o comando de teste definido no `CLAUDE.md` ou fornecido pelo usuário.

### Interprete o resultado

**Se passou (verde):**
- Mostre o output do teste confirmando que passou
- Declare explicitamente: "✅ Teste passou."
- Não faça mais nada

**Se ainda falhou:**
- Mostre o erro exato do output
- Analise a causa (lógica errada, path errado, import faltando)
- Corrija **somente** o que está causando a falha
- Rode novamente
- Repita até passar (máximo 3 tentativas antes de pedir ajuda ao usuário)

---

## Step 5: Output final

Entregue:
1. O arquivo de produção criado/modificado (conteúdo completo)
2. O output do comando de teste mostrando verde
3. Nada mais

**Não sugira refatorações.** Não diga "poderíamos melhorar X". Isso é fase seguinte (refactor).

---

## Edge cases

| Situação | Ação |
|---|---|
| Usuário não informou o arquivo de teste | Pergunte qual arquivo ou encontre pelo padrão do projeto |
| Comando de teste não está no CLAUDE.md | Pergunte ao usuário antes de executar |
| Teste tem múltiplos `it`/`test` e alguns já passavam | Não quebre os que já passavam — rode a suíte completa e confirme todos verdes |
| Usuário pede para "também refatorar" | Recuse educadamente: refactor é fase separada após o verde confirmado |
| Múltiplos arquivos de produção necessários | Crie todos, mas com o mínimo em cada um |
| Teste usa mock de dependência externa | Implemente só a interface/contrato que o mock espera, não a dependência real |