import { useState, useEffect, useRef } from 'react';
import { Outlet, Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { 
  LayoutDashboard, 
  Users, 
  UserCog, 
  LogOut, 
  Menu,
  X,
  Plus
} from 'lucide-react';
import { useTranslation } from 'react-i18next';
import toast from 'react-hot-toast';
import api from '../api/axios';
import LanguageSelector from './LanguageSelector';

export default function AdminLayout() {
  const { t } = useTranslation();
  const { user, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  
  const [profileImage, setProfileImage] = useState(null);
  const [imageLoading, setImageLoading] = useState(true);
  const [actualEmployeeId, setActualEmployeeId] = useState(null);
  const fileInputRef = useRef(null);

  useEffect(() => {
    const fetchEmployeeDetails = async () => {
      if (user?.employeeId) {
        try {
          const res = await api.get(`/employees/id/${user.employeeId}`);
          if (res.data?.success !== false && res.data?.data?.employeeId) {
            setActualEmployeeId(res.data.data.employeeId);
          } else {
             setImageLoading(false);
          }
        } catch (error) {
          console.error("Failed to fetch employee details", error);
          setImageLoading(false);
        }
      } else {
        setImageLoading(false);
      }
    };
    fetchEmployeeDetails();
  }, [user]);

  const fetchProfileImage = async (empId) => {
    if (!empId) return;
    try {
      setImageLoading(true);
      const response = await api.get(`/profile/${empId}`, {
        responseType: 'blob'
      });
      const imageUrl = URL.createObjectURL(response.data);
      setProfileImage(imageUrl);
    } catch (error) {
      setProfileImage(null);
    } finally {
      setImageLoading(false);
    }
  };

  useEffect(() => {
    if (actualEmployeeId) {
      fetchProfileImage(actualEmployeeId);
    }
  }, [actualEmployeeId]);

  const handleImageUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;
    if (!actualEmployeeId) {
      toast.error('Missing employee ID. Please log out and log in again.');
      return;
    }

    const formData = new FormData();
    formData.append('EmployeeId', actualEmployeeId);
    formData.append('Image', file);

    try {
      setImageLoading(true);
      await api.post('/profile/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      toast.success('Profile image updated');
      fetchProfileImage(actualEmployeeId);
    } catch (error) {
      toast.error('Failed to upload image');
      setImageLoading(false);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navItems = [
    { name: t('layout.dashboard'), path: '/admin/dashboard', icon: LayoutDashboard },
    { name: t('layout.employees'), path: '/admin/employees', icon: Users, roles: ['SuperAdmin'] },
    { name: t('layout.users'), path: '/admin/users', icon: UserCog, roles: ['Admin', 'SuperAdmin'] },
  ];

  const filteredNav = navItems.filter(item => !item.roles || item.roles.some(r => user?.roles?.includes(r)));

  return (
    <div className="min-h-screen bg-gray-50 flex">
      {/* Mobile sidebar overlay */}
      {isSidebarOpen && (
        <div 
          className="fixed inset-0 bg-gray-900/50 z-20 lg:hidden"
          onClick={() => setIsSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <div className={`
        fixed inset-y-0 left-0 z-30 w-64 bg-cbe-purple text-white transform transition-transform duration-300 ease-in-out lg:translate-x-0 lg:static lg:inset-0
        ${isSidebarOpen ? 'translate-x-0' : '-translate-x-full'}
      `}>
        <div className="flex items-center justify-between h-16 px-6 bg-cbe-dark-purple">
          <span className="text-xl font-bold text-white tracking-wide">{t('layout.title')}</span>
          <button className="lg:hidden text-gray-300 hover:text-white" onClick={() => setIsSidebarOpen(false)}>
            <X size={20} />
          </button>
        </div>
        
        <div className="p-4 border-b border-cbe-purple/50 flex items-center gap-4">
          <div className="relative group">
            <div className="w-12 h-12 rounded-full overflow-hidden bg-cbe-purple/50 flex items-center justify-center border-2 border-white/20 shrink-0">
              {imageLoading ? (
                <div className="w-4 h-4 border-2 border-white/50 border-t-white rounded-full animate-spin"></div>
              ) : profileImage ? (
                <img src={profileImage} alt="Profile" className="w-full h-full object-cover" />
              ) : (
                <span className="text-gray-300 font-medium text-lg">
                  {user?.email?.charAt(0).toUpperCase() || 'U'}
                </span>
              )}
            </div>
            
            <button 
              onClick={() => fileInputRef.current?.click()}
              className="absolute bottom-0 right-0 w-5 h-5 bg-cbe-gold rounded-full flex items-center justify-center border-2 border-cbe-dark-purple text-cbe-dark-purple hover:bg-yellow-400 transition-colors shadow-sm"
              title="Upload Profile Image"
            >
              <Plus size={12} strokeWidth={3} />
            </button>
            <input 
              type="file" 
              ref={fileInputRef} 
              onChange={handleImageUpload} 
              accept="image/*" 
              className="hidden" 
            />
          </div>
          <div className="min-w-0">
            <p className="text-sm text-gray-300">{t('layout.welcome')}</p>
            <p className="font-medium truncate" title={user?.email}>{user?.email}</p>
          </div>
        </div>

        <nav className="mt-6 px-3 space-y-1">
          {filteredNav.map((item) => {
            const Icon = item.icon;
            const isActive = location.pathname.startsWith(item.path);
            return (
              <Link
                key={item.name}
                to={item.path}
                className={`
                  flex items-center px-3 py-2.5 rounded-lg text-sm font-medium transition-colors
                  ${isActive ? 'bg-white/10 text-cbe-gold' : 'text-gray-300 hover:bg-white/5 hover:text-white'}
                `}
                onClick={() => setIsSidebarOpen(false)}
              >
                <Icon className={`mr-3 h-5 w-5 flex-shrink-0 ${isActive ? 'text-cbe-gold' : 'text-gray-400'}`} />
                {item.name}
              </Link>
            );
          })}
        </nav>
      </div>

      {/* Main Content */}
      <div className="flex-1 flex flex-col min-w-0 overflow-hidden">
        {/* Top Header */}
        <header className="bg-white shadow-sm h-16 flex items-center justify-between px-4 sm:px-6 lg:px-8 z-10">
          <div className="flex items-center">
            <button
              className="lg:hidden p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 focus:outline-none"
              onClick={() => setIsSidebarOpen(true)}
            >
              <Menu className="h-6 w-6" />
            </button>
            <h1 className="ml-4 lg:ml-0 text-xl font-semibold text-gray-800">
              {filteredNav.find(n => location.pathname.startsWith(n.path))?.name || t('layout.dashboard')}
            </h1>
          </div>
          
          <div className="flex items-center gap-4">
            <LanguageSelector />
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 text-sm font-medium text-gray-500 hover:text-red-600 transition-colors"
            >
              <LogOut size={18} />
              <span className="hidden sm:inline">{t('layout.logout')}</span>
            </button>
          </div>
        </header>

        {/* Page Content */}
        <main className="flex-1 overflow-y-auto p-4 sm:p-6 lg:p-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
