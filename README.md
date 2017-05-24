# biv-parser

**B**lock **I**nstruction **V**ariable parser

## Introduction

The purpose of this repository is to create an algorithm using C#.NET that parses to following file structure:

```
BlockA
{
  m_variableA = 20;
  m_variableB = "Hello world!";
  InstructionA(string_param, 12);
  OtherInstruction
  (
    "Parameter",
    0
  );
  
  BlockB
  {
    m_variableC = 42;
    InstructionB();
  }
}
```

In this example, `BlockA` contains *variables* (`m_variableA` and `m_variableB`), *instructions* (`InstructionA` with `string_param` as a string and `12` as an integer) and also another *block* (`BlockB`) that contains stuff in it.

Every variable or instructions **should** be inside a block.

## Structure

By reading the example above, we can see that a *block* can contain *variables*, *instructions* and *blocks*.

*variables*, *instructions* and *blocks* can be considered as statements.

First all our statements, will have a name and a type.

### IStatement

| Field | Type | Description |
|-------|------|-------------|
| Name | `string` | Name of the statement |
| Type | `Enum` | Type of the statement (`Block`, `Instruction` or `Variable`) |

Let's define these three statements:

### Variable : IStatement

| Field | Type | Description |
|-------|------|-------------|
| Value | `object` | Value of the variable |

### Instruction : IStatement

| Field | Type | Description |
|-------|------|-------------|
| Parameters | `object[]` | Array containing the instruction parameters |

### Block : IStatement

| Field | Type | Description |
|-------|------|-------------|
| Statements | `List<IStatement>` | List of the statements inside this one. (Blocks, instructions and variables) |
| this[`string`] | `Block` | Access the block match the indexer name. |

## Performance

### Reading and ignore comments

File size: 1MB
Read Time: 150~200 ms
Number of tokens: 63108

## Parsing

File size: 1MB
Number of tokens: 63108
Parse time: (xx) ms
Number of blocks:
Number of instructions:
Number of variables
