const FONT_KEY = 'font-family';

export const FONT_PRESETS = [
  { label: 'System default', value: "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif" },
  { label: 'Serif', value: "Georgia, 'Times New Roman', serif" },
  { label: 'Monospace', value: "'Cascadia Code', Consolas, monospace" },
  { label: 'Rounded', value: "'Comic Sans MS', 'Comic Sans', cursive" },
] as const;

export function applyStoredFontPreference(): void {
  const stored = localStorage.getItem(FONT_KEY);
  if (stored) {
    document.documentElement.style.setProperty('--font-family', stored);
  }
}

export function setFontPreference(value: string): void {
  document.documentElement.style.setProperty('--font-family', value);
  localStorage.setItem(FONT_KEY, value);
}

export function getFontPreference(): string {
  return localStorage.getItem(FONT_KEY) ?? FONT_PRESETS[0].value;
}
