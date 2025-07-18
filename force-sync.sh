#!/bin/bash

echo "⚠️  Este script eliminará todos los cambios locales y sobrescribirá tu copia con la versión de GitHub."
read -p "¿Estás seguro de continuar? (s/n): " confirm

if [[ "$confirm" != "s" && "$confirm" != "S" ]]; then
  echo "❌ Operación cancelada."
  exit 1
fi

echo "🚀 Sincronizando con GitHub..."
git fetch origin

echo "🔄 Forzando reset a origin/main..."
git reset --hard origin/main

echo "🧹 Limpiando archivos no registrados..."
git clean -fd

echo "✅ Proyecto sincronizado completamente con GitHub."
