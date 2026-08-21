import { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { Lock, Mail, ArrowLeft } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import LanguageSelector from '../../components/LanguageSelector';
import { useEffect } from 'react';
import api from '../../api/axios';

export default function Login() {
  const { t } = useTranslation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  useEffect(() => {
    try {
      const response = api.get("/users/RSAPublicKey").then((response) => {
        console.log(response.data)
      })
    } catch (error) {
      console.log(error)
    }
  }, [])

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await login(email, password);
      toast.success(t('login.success'));
      navigate('/admin/dashboard');
    } catch (error) {
      toast.error(error.response?.data?.message || t('kiosk.error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-cbe-light flex flex-col justify-center items-center p-4 relative">
      <div className="absolute top-6 left-6 right-6 flex items-center justify-between">
        <button
          onClick={() => navigate('/')}
          className="flex items-center gap-2 text-gray-600 hover:text-cbe-purple font-medium transition-colors"
        >
          <ArrowLeft size={20} />
          <span>{t('login.backToKiosk')}</span>
        </button>
        <LanguageSelector />
      </div>

      <div className="w-full max-w-md bg-white rounded-2xl shadow-xl overflow-hidden border-t-[6px] border-cbe-purple">
        <div className="p-8 pb-6 text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">{t('login.title')}</h2>
          <p className="text-gray-500 text-sm">{t('login.subtitle')}</p>
        </div>

        <form onSubmit={handleLogin} className="p-8 pt-0 space-y-5">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('login.emailLabel')}</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <Mail className="h-5 w-5 text-gray-400" />
              </div>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="block w-full pl-10 pr-3 py-2.5 border border-gray-300 rounded-lg focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm"
                placeholder="admin@cbe.com.et"
                required
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('login.passwordLabel')}</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <Lock className="h-5 w-5 text-gray-400" />
              </div>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="block w-full pl-10 pr-3 py-2.5 border border-gray-300 rounded-lg focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm"
                placeholder="••••••••"
                required
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className={`w-full py-2.5 px-4 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white 
              ${loading ? 'bg-cbe-purple/70 cursor-not-allowed' : 'bg-cbe-purple hover:bg-cbe-dark-purple'} 
              transition-colors duration-200`}
          >
            {loading ? t('login.buttonLoading') : t('login.button')}
          </button>
        </form>
      </div>
    </div>
  );
}
