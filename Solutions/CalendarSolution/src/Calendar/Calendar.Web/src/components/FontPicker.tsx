import { useState } from 'react';
import { FONT_PRESETS, getFontPreference, setFontPreference } from '../lib/fontPreference';

export function FontPicker() {
  const [value, setValue] = useState(getFontPreference());

  return (
    <label className="flex items-center gap-2 text-xs text-gray-500">
      Font
      <select
        className="rounded border border-gray-300 bg-white px-2 py-1 text-xs text-gray-700"
        value={value}
        onChange={(e) => {
          setValue(e.target.value);
          setFontPreference(e.target.value);
        }}
      >
        {FONT_PRESETS.map((preset) => (
          <option key={preset.label} value={preset.value}>
            {preset.label}
          </option>
        ))}
      </select>
    </label>
  );
}
