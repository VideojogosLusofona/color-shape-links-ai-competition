# Thinker implementation guide {#thinker-implementation-guide}

@brief A guide on how to implement an AI thinker

[TOC]

## Introduction

Basic stuff already in the quick start

## Rules for the AI thinkers

The source code of AI thinkers must follow these rules:

- Can only use cross-platform [.NET Standard 2.0] API calls in C#.
- Can use additional libraries which abide by these same rules.
- Both the AI code and libraries must be made available under a
  [valid open source license][ossl], although AI codes can be open-sourced
  only after the competition deadline.
- Must run in the same process that invokes it.
- Can be multithreaded and use [`unsafe`] contexts.
- Cannot *think* in its opponent time (e.g., by using a background thread).
- Must acknowledge [cancellation requests][`CancellationToken`], in which case
  it should terminate quickly and in an orderly fashion.
- Can only probe the environment for the number of processor cores. It cannot
  search or use any other information, besides what is already available in the
  [`AbstractThinker`] class or passed to the [`Think()`] method, e.g., such as
  using reflection to probe the capabilities of its opponents.
- Cannot use more than 2GB of memory during the course of a match.
- Cannot be more than 250kb in size (including libraries, excluding comments).
- Cannot save or load data from disk.

## Implementing an AI thinker

### Setting up the development environment

### Implementing a simple Minimax player

Steps to create a minimax player

## Testing an AI thinker in the console app

TODO: The quick way is already explained. Here we discuss proper development,
with separate folder/repo.

TODO: Run match/session with external assembly/thinker.

## Testing the AI thinker in isolation

TODO: Warn about thinker variables not being available when thinker is
instantiated directly.