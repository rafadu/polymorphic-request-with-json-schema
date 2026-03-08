---
name: tdd-write-test
description: >
  Use this skill whenever the user asks to write a unit test that should fail, write only the test (not production code), 
  follow TDD red-phase, or says things like "escreva o teste que falha", "only the test", "don't write the implementation",
  "red phase", "test first". This skill enforces strict TDD discipline: write the failing test only, never the production code.
  Trigger even if the user just says "write a test for X" without explicitly saying TDD — the skill will clarify intent.
---

# TDD Failing Test Skill

Guides Claude to write **only** the failing unit test (red phase of TDD). Production code is **never** written.

---

## Step 1: Read CLAUDE.md

Before writing anything, read `CLAUDE.md` in the project root. It defines:

- **Language / framework** (e.g., TypeScript, Python, Go, Java)
- **Test runner** (e.g., Jest, Pytest, JUnit, Go test)
- **Project conventions** (naming, folder structure, import style)
- **Domain context** (what the system does)

If `CLAUDE.md` is missing or incomplete, ask the user for the missing info before proceeding.

---

## Step 2: Understand what to test

Identify the behavior to test from:
1. The file the user pointed to (read it to understand its interface/signature)
2. Or the description the user gave

Do **not** assume implementation details. Focus on **observable behavior** (inputs → outputs, side effects, thrown errors).

---

## Step 3: Write the failing test

### Rules (non-negotiable)

- ✅ Write **only** the test file
- ✅ The test **must fail** when run (because production code doesn't exist yet, or the behavior isn't implemented)
- ✅ Import/reference the production module at its **expected path** (the path it will have once created)
- ✅ Use the test runner and assertion style defined in `CLAUDE.md`
- ❌ Never write the production code
- ❌ Never make the test pass
- ❌ Never add `// TODO: implement` stubs inside the test
- ❌ Never mock the module-under-test itself (mocking dependencies is fine)

### What makes a good failing test

- Tests **one behavior** per test case
- Has a clear **Arrange / Act / Assert** structure (or Given/When/Then)
- The failure reason is obvious from the assertion message
- Uses descriptive test names: `should return total price including tax`

---

## Step 4: Verify the test will fail

After writing the test, briefly explain **why it will fail**:

- "This test will fail because `src/myModule.ts` does not exist yet."
- "This test will fail because `calculateTotal` is not implemented."
- "This test will fail because the function currently returns `undefined`."

Do **not** run the test unless the user explicitly asks. Just state the expected failure reason.

---

## Step 5: Output format

Deliver:
1. The test file content (full, ready to save)
2. The **expected file path** for the test
3. A one-line explanation of why it fails

Nothing else. No production code. No implementation hints beyond what's needed to understand the test.

---

## Edge cases

| Situation | Action |
|---|---|
| File to test doesn't exist yet | Import from expected future path; test fails on import |
| File exists but method is missing | Import file, call missing method; test fails on call |
| User asks "also write the implementation" | Decline politely, explain this skill is red-phase only; suggest they ask separately after |
| CLAUDE.md missing | Ask user: language, test runner, and where test files live |
| Multiple behaviors to test | Write one `describe` block with multiple `it`/`test` cases, one per behavior |