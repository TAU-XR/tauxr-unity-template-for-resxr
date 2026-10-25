# Maintaining the studio template

This file is for whoever maintains `tauxr-unity-template-for-resxr` itself. Delete it from projects created from the template.

## How the pieces relate

```
ResXR/resxr-unity-research-template      (public, owned by ResXR — we never change it)
        │  occasional, manual merge
        ▼
TAU-XR/tauxr-unity-template-for-resxr    (this repo — studio template: new versions + TAUXR changes)
        │  "Use this template" on GitHub (one-time snapshot)
        ▼
TAU-XR/<each new project>
```

- ResXR stays untouched and available for everyone. Studio changes live only here.
- A project is a snapshot of this template on the day it was created. Improving the template later does **not**
  change existing projects.
- To improve the template: edit it in this repo, test, commit, push. Every project created afterwards gets it.

## Pulling an update from ResXR

You only need this if ResXR releases something you want (a bug fix, a new feature). Most of the time you won't.

**One-time setup on a new machine** (the remote isn't stored in the repo):

```bash
git remote add upstream https://github.com/ResXR/resxr-unity-research-template.git
git remote set-url --push upstream DISABLED
```

**Optional — Unity Smart Merge** (merges scenes and prefabs much better than plain git; `.gitattributes` already
points `.unity`, `.prefab`, `.asset`, `.meta` etc. at it). Run once per machine:

```bash
git config --global merge.unityyamlmerge.name "Unity SmartMerge"
git config --global merge.unityyamlmerge.driver "'C:/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Data/Tools/UnityYAMLMerge.exe' merge -p %O %B %A %A"
git config --global merge.unityyamlmerge.recursive binary
```

Without it git simply uses its normal merge for those files.

**Each update:**

```bash
git fetch upstream
git log --oneline main..upstream/main      # see what's new in ResXR
git merge upstream/main
```

Git will probably stop with conflicts. Expect them in the files we deliberately changed:

| File(s) | What to do |
| ------- | ---------- |
| `Packages/manifest.json`, `Packages/packages-lock.json` | Keep **ours** (our SDK/Unity versions). If ResXR added a new package, add that line by hand. Then let Unity regenerate the lock file. |
| `ProjectSettings/*` | Keep **ours**: `git checkout --ours ProjectSettings/ && git add ProjectSettings/` |
| ResXR scripts we changed (list in the README, "Changes from ResXR") | Open the file and combine: keep our fix *and* their change. |
| Scenes / prefabs (`.unity`, `.prefab`) | Usually keep **ours** and re-apply their change in the Unity editor — merging scene YAML by hand is error-prone. |

Then:
1. Open the project in Unity 6000.3.24f1, fix any compile errors (ResXR is developed on an older SDK).
2. Run the Base Scene on a Quest.
3. `git commit` (finishes the merge) and `git push`.

**Taking a single fix instead of a whole release:**

```bash
git fetch upstream
git log --oneline upstream/main            # find the commit you want
git cherry-pick <commit-hash>
```

If a merge goes wrong before you commit, `git merge --abort` puts everything back.

## Upgrading Unity or the Meta SDK

1. Check Meta's release notes for the minimum Unity version.
2. Change the versions in `Packages/manifest.json` (Meta packages: core, mrutilitykit, haptics, interaction.ovr,
   platform share one version number; audio and voice have their own).
3. Open in the new Unity editor, fix compile errors, test on a Quest.
4. Update the versions in `README.md` and `STUDIO_SETUP.md`.
