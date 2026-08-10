import { useTranslation } from 'react-i18next';
import { Globe } from 'lucide-react';

export default function LanguageSelector() {
  const { i18n } = useTranslation();

  const changeLanguage = (e) => {
    i18n.changeLanguage(e.target.value);
  };

  const currentLang = i18n.language?.split('-')[0] || 'en';

  return (
    <div className="flex items-center gap-2 px-3 py-1.5 text-sm font-medium text-gray-600 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors border border-gray-200 focus-within:ring-2 focus-within:ring-cbe-purple/50">
      <Globe size={16} className="text-cbe-purple shrink-0" />
      <select
        value={currentLang}
        onChange={changeLanguage}
        className="bg-transparent outline-none cursor-pointer text-gray-600 w-full"
        title="Select Language"
      >
        <option value="en">English</option>
        <option value="am">አማርኛ</option>
        <option value="or">Afaan Oromoo</option>
        <option value="ti">ትግርኛ</option>
      </select>
    </div>
  );
}