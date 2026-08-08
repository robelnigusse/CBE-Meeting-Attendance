import { useState } from 'react';
import api from '../../api/axios';
import toast from 'react-hot-toast';
import { LogIn, UserCheck } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import LanguageSelector from '../../components/LanguageSelector';

export default function Kiosk() {
  const { t } = useTranslation();
  const [employeeId, setEmployeeId] = useState('');
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState(null); // 'success' or 'error'
  const navigate = useNavigate();

  const handleCheckIn = async (e) => {
    e.preventDefault();
    if (!employeeId) return;

    setLoading(true);
    setStatus(null);
    try {
      // API call to take attendance
      const response = await api.post('/attendance', { employeeId });
      setStatus('success');
      toast.success(response.data.message || t('kiosk.success'));
      setEmployeeId('');
      
      // Reset status after a few seconds
      setTimeout(() => setStatus(null), 3000);
    } catch (error) {
      setStatus('error');
      toast.error(error.response?.data?.message || t('kiosk.error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-cbe-light flex flex-col justify-center items-center p-4 relative">
      {/* Admin Login Link */}
      <div className="absolute top-6 right-6 flex items-center gap-4">
        <LanguageSelector />
        <button 
          onClick={() => navigate('/login')}
          className="flex items-center gap-2 text-cbe-purple hover:text-cbe-dark-purple font-medium"
        >
          <LogIn size={20} />
          <span>{t('kiosk.adminPortal')}</span>
        </button>
      </div>

      <div className="w-full max-w-md bg-white rounded-2xl shadow-xl overflow-hidden">
        {/* Header */}
        <div className="bg-cbe-purple p-8 text-center">
          <h1 className="text-3xl font-bold text-white mb-2">{t('kiosk.title')}</h1>
          <p className="text-cbe-gold font-medium">{t('kiosk.subtitle')}</p>
        </div>

        {/* Body */}
        <div className="p-8">
          <form onSubmit={handleCheckIn} className="space-y-6">
            <div>
              <label htmlFor="employeeId" className="block text-sm font-medium text-gray-700 mb-2">
                {t('kiosk.employeeIdLabel')}
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <UserCheck className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type="text"
                  id="employeeId"
                  value={employeeId}
                  onChange={(e) => setEmployeeId(e.target.value)}
                  className="block w-full pl-10 pr-3 py-3 border border-gray-300 rounded-lg focus:ring-cbe-purple focus:border-cbe-purple text-lg"
                  placeholder={t('kiosk.placeholder')}
                  required
                />
              </div>
            </div>

            <button
              type="submit"
              disabled={loading || !employeeId}
              className={`w-full py-3 px-4 border border-transparent rounded-lg shadow-sm text-lg font-medium text-white 
                ${loading || !employeeId ? 'bg-gray-400 cursor-not-allowed' : 'bg-cbe-gold hover:bg-[#d18f23] hover:shadow-lg'} 
                transition-all duration-200`}
            >
              {loading ? t('kiosk.buttonProcessing') : t('kiosk.button')}
            </button>
          </form>

          {/* Status Messages */}
          {status === 'success' && (
            <div className="mt-6 p-4 bg-green-50 rounded-lg border border-green-200 text-center text-green-700">
              {t('kiosk.success')}
            </div>
          )}
          {status === 'error' && (
            <div className="mt-6 p-4 bg-red-50 rounded-lg border border-red-200 text-center text-red-700">
              {t('kiosk.error')}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
