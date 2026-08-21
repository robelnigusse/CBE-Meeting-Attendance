import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import enTranslations from './locales/en/translations.json';
import amTranslations from './locales/am/translations.json';
import orTranslations from './locales/or/translations.json';
import tiTranslations from './locales/ti/translations.json';

i18n.use(initReactI18next).init({
  fallbackLng: 'en',
  lng: 'en',
  resources: {
    en: { translations: enTranslations },
    am: { translations: amTranslations },
    or: { translations: orTranslations },
    ti: { translations: tiTranslations }
  },
  ns: ['translations'],
  defaultNS: 'translations'
});

i18n.languages = ['en', 'am', 'or', 'ti'];

export default i18n;