# LiveSplit.Salt
Salt and Sanctuary autosplitter for LiveSplit.

The original autosplitter did not work anymore. This is my attempt to fix it.

## Summary
- [LiveSplit.Salt](#livesplitsalt)
  - [Summary](#summary)
  - [Installation](#installation)
    - [Automated](#automated)
    - [Hands on](#hands-on)
  - [Features](#features)
  - [Ideas that may or may not see the light](#ideas-that-may-or-may-not-see-the-light)
  - [Known issues](#known-issues)
    - [The run auto-starts, but doesn't autosplit](#the-run-auto-starts-but-doesnt-autosplit)
    - [When starting a new run, it autosplits until the last split.](#when-starting-a-new-run-it-autosplits-until-the-last-split)

## Installation

### Automated
If I did everything right, Livesplit should suggest you to use it automatically. It will
take care of the download and installation for you. Just click `Activate` and you're done.

### Hands on
If your Livesplit can't find it, you can download the [Livesplit.Salt/Components/
LiveSplit.Salt.dll](https://github.com/vorban/LiveSplit.Salt/blob/master/Livesplit.Salt/Components/LiveSplit.Salt.dll) file.

Put it in your LiveSplit's Components folder (`C:\Path\To\My\Livesplit\Components\`).
Reopen Livesplit and it should be available.

## Features
- Auto start
- Autosplit on each boss except Nameless God
- Randomize character appearance on each split (optional)
- Splits when finishing the game (after Nameless God)

## Ideas that may or may not see the light
- Option to split on nameless king
- Maybe use actual boss health to split (instead of item drop)
- Randomize after character creation
- Option to split on specific item/spell loot/use, and on gameload.

## Known issues

### The run auto-starts, but doesn't autosplit

This has been fixed in 0.0.6. Make sure your autosplitter is up-to-date.
### When starting a new run, it autosplits until the last split.

This has been fixed in 0.1.0. Make sure your autosplitter is up-to-date.
