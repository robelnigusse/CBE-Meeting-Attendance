import { useState, useEffect } from 'react';
import api from '../../api/axios';
import { Users, UserCheck, Sun, Moon, Download } from 'lucide-react';
import toast from 'react-hot-toast';
import { useTranslation } from 'react-i18next';

export default function Dashboard() {
  const { t } = useTranslation();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [exporting, setExporting] = useState(false);

  const fetchDashboard = async () => {
    try {
      const response = await api.get('/admin/dashboard');
      setStats(response.data.data);
    } catch (error) {
      toast.error('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDashboard();
  }, []);

  const handleExport = async (format) => {
    setExporting(true);
    try {
      const response = await api.get(`/admin/export/${format}`, { responseType: 'blob' });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      
      const ext = format === 'excel' ? 'xlsx' : format;
      link.setAttribute('download', `CBE_Attendance_Report.${ext}`);
      
      document.body.appendChild(link);
      link.click();
      link.remove();
      toast.success(`Exported ${format.toUpperCase()} successfully`);
    } catch (error) {
      toast.error(`Failed to export ${format}`);
    } finally {
      setExporting(false);
    }
  };

  if (loading) {
    return <div className="flex justify-center items-center h-64"><div className="animate-pulse flex space-x-4"><div className="rounded-full bg-slate-200 h-10 w-10"></div><div className="flex-1 space-y-6 py-1"><div className="h-2 bg-slate-200 rounded"></div></div></div></div>;
  }

  const statCards = [
    { title: t('dashboard.totalEmployees'), value: stats?.totalEmployees || 0, icon: Users, color: 'text-blue-600', bg: 'bg-blue-100' },
    { title: t('dashboard.todayAttendance'), value: stats?.todayAttendance || 0, icon: UserCheck, color: 'text-green-600', bg: 'bg-green-100' },
    { title: t('dashboard.morning'), value: stats?.morningAttendance || 0, icon: Sun, color: 'text-amber-500', bg: 'bg-amber-100' },
    { title: t('dashboard.afternoon'), value: stats?.afternoonAttendance || 0, icon: Moon, color: 'text-cbe-purple', bg: 'bg-purple-100' },
  ];

  return (
    <div className="space-y-6">
      {/* Top Stats */}
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {statCards.map((item) => {
          const Icon = item.icon;
          return (
            <div key={item.title} className="bg-white overflow-hidden shadow rounded-lg border border-gray-100">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className={`rounded-md p-3 ${item.bg}`}>
                      <Icon className={`h-6 w-6 ${item.color}`} aria-hidden="true" />
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">{item.title}</dt>
                      <dd className="text-2xl font-semibold text-gray-900">{item.value}</dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Export Section & Recent Attendees */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white shadow rounded-lg p-6 border border-gray-100 lg:col-span-2">
          <h2 className="text-lg font-medium text-gray-900 mb-4">{t('dashboard.todayAttendees')}</h2>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{t('dashboard.employee')}</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{t('dashboard.dateAndTime')}</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{t('dashboard.status')}</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {stats?.todayAttendees?.length > 0 ? (
                  stats.todayAttendees.map((attendee, idx) => (
                    <tr key={idx} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-gray-900">{attendee.fullName}</div>
                        <div className="text-sm text-gray-500">{attendee.employeeId}</div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        <div className="text-gray-900">{new Date(attendee.attendanceTime).toLocaleDateString()}</div>
                        <div>{new Date(attendee.attendanceTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} ({attendee.session})</div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                          {t('dashboard.present')}
                        </span>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="3" className="px-6 py-10 text-center text-sm text-gray-500">
                      {t('dashboard.noAttendees')}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Actions Sidebar */}
        <div className="bg-white shadow rounded-lg p-6 border border-gray-100">
          <h3 className="text-lg font-medium text-gray-900 mb-4">{t('dashboard.exportReports')}</h3>
          <div className="space-y-3">
            <button
              onClick={() => handleExport('csv')}
              disabled={exporting}
              className="w-full flex justify-center items-center gap-2 py-2.5 px-4 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-cbe-purple"
            >
              <Download size={16} /> {t('dashboard.csv')}
            </button>
            <button
              onClick={() => handleExport('excel')}
              disabled={exporting}
              className="w-full flex justify-center items-center gap-2 py-2.5 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              <Download size={16} /> {t('dashboard.excel')}
            </button>
            <button
              onClick={() => handleExport('pdf')}
              disabled={exporting}
              className="w-full flex justify-center items-center gap-2 py-2.5 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
            >
              <Download size={16} /> {t('dashboard.pdf')}
            </button>
          </div>
        </div>

      </div>
    </div>
  );
}
