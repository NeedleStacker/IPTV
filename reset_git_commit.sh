#!/bin/bash

# Provjeri da li je git repozitorij
if [ ! -d ".git" ]; then
  echo "❌ Nije prepoznat git repozitorij. Pokreni ovu skriptu unutar git repozitorija."
  exit 1
fi

#unijeti commit ID na koji se treba forsirati 
TARGET_COMMIT="7641fa1caae29d1a810a221311b28f78d3d9c9d6"

echo "🔄 Vraćam repozitorij na commit $TARGET_COMMIT ..."

# Checkout commit
git checkout $TARGET_COMMIT || exit 1

# Reset granu na commit (ovo briše lokalne promjene)
git reset --hard $TARGET_COMMIT

# Forsiraj promjene na remote (ako želiš da se odrazi na GitHub)
read -p "Želiš li pushati promjene na remote (ovo će prepisati history)? [y/N]: " choice

if [[ "$choice" == "y" || "$choice" == "Y" ]]; then
  # Detektiraj aktivnu granu (ako je nema, traži od korisnika)
current_branch=$(git branch --show-current)

if [ -z "$current_branch" ]; then
  echo "⚠️ Nisi na grani (detached HEAD)."
  read -p "Unesi ime grane na koju zelis pushati (npr. main): " target_branch
else
  target_branch=$current_branch
fi

echo "⬆️  Forsiram promene na granu '$target_branch'..."
git push origin HEAD:refs/heads/$target_branch --force || {
  echo "❌ Push nije uspeo. Proveri naziv grane ili prava pristupa."
  exit 1
}
echo "✅ Promjene su forsirane na remote granu '$target_branch'."

  echo "✅ Promjene su forsirane na remote."
else
  echo "ℹ️ Promjene nisu poslane na remote."
fi

echo "✅ Repozitorij je vraćen na commit $TARGET_COMMIT."
