import { useState, useEffect } from 'react';
import api from '../../api/axios';
import toast from 'react-hot-toast';
import { Plus, Trash2, X, Shield, Key } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';

export default function Users() {
  const { user: currentUser } = useAuth();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    employeeId: ''
  });

  const fetchUsers = async () => {
    try {
      const response = await api.get('/users');
      setUsers(response.data.data || []);
    } catch (error) {
      toast.error('Failed to load users');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const openModal = () => {
    setFormData({ email: '', password: '', employeeId: '' });
    setIsModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post('/users/register', formData);
      if (response.data.success !== false) {
        toast.success('User registered successfully');
        setIsModalOpen(false);
        fetchUsers();
      } else {
        toast.error(response.data.message || 'Error registering user');
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Error registering user');
    }
  };

  const handleRoleChange = async (userId, newRole) => {
    try {
      const response = await api.put('/users/change-role', { userId, newRole });
      if (response.data.success !== false) {
        toast.success('Role updated successfully');
        fetchUsers();
      } else {
        toast.error(response.data.message || 'Error changing role');
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Error changing role');
    }
  };

  return (
    <div className="space-y-6">
      <div className="sm:flex sm:items-center sm:justify-between">
        <div>
          <h2 className="text-lg font-medium text-gray-900">System Users</h2>
          <p className="mt-1 text-sm text-gray-500">
            Administrators who have access to manage the system.
          </p>
        </div>
        <div className="mt-4 sm:mt-0">
          <button
            onClick={openModal}
            className="flex items-center justify-center rounded-md border border-transparent bg-cbe-purple px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-cbe-dark-purple focus:outline-none focus:ring-2 focus:ring-cbe-purple focus:ring-offset-2 sm:w-auto"
          >
            <Plus className="mr-2 h-4 w-4" />
            Add Admin User
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="bg-white shadow rounded-lg border border-gray-100 overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading users...</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email / Account</th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Roles</th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Linked Employee ID</th>
                  <th scope="col" className="relative px-6 py-3"><span className="sr-only">Actions</span></th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {users.map((u) => (
                  <tr key={u.id || u.email} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 flex items-center gap-2">
                      <Shield className="h-4 w-4 text-cbe-gold" />
                      {u.email}
                      {currentUser?.email === u.email && (
                        <span className="ml-2 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800">You</span>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {u.roles?.map(role => (
                        <span key={role} className="mr-1 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                          {role}
                        </span>
                      ))}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {u.employeeId || '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      {currentUser?.roles?.includes('SuperAdmin') && u.email !== currentUser.email && (
                        <select 
                          className="mr-4 border-gray-300 rounded-md text-sm focus:ring-cbe-purple focus:border-cbe-purple"
                          value={u.roles?.[0] || 'Staff'}
                          onChange={(e) => handleRoleChange(u.id, e.target.value)}
                        >
                          <option value="Staff">Staff</option>
                          <option value="Admin">Admin</option>
                          <option value="SuperAdmin">SuperAdmin</option>
                        </select>
                      )}
                      <button className="text-gray-400 hover:text-gray-500 cursor-not-allowed" title="Reset Password coming soon">
                        <Key size={16} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Add User Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
          <div className="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
            <div className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" onClick={() => setIsModalOpen(false)}></div>
            <span className="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
            <div className="relative z-10 inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-md sm:w-full">
              <div className="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4 border-t-4 border-cbe-purple">
                <div className="flex justify-between items-center mb-4">
                  <h3 className="text-lg leading-6 font-medium text-gray-900" id="modal-title">Register New Admin</h3>
                  <button onClick={() => setIsModalOpen(false)} className="text-gray-400 hover:text-gray-500"><X size={20} /></button>
                </div>
                
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Email Address <span className="text-red-500">*</span></label>
                    <input type="email" name="email" required value={formData.email} onChange={handleInputChange}
                      className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Password <span className="text-red-500">*</span></label>
                    <input type="password" name="password" required value={formData.password} onChange={handleInputChange}
                      className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                    />
                    <p className="text-xs text-gray-500 mt-1">Must be at least 6 characters, with 1 uppercase and 1 lowercase letter.</p>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Linked Employee ID <span className="text-red-500">*</span></label>
                    <input type="text" name="employeeId" required value={formData.employeeId} onChange={handleInputChange} placeholder="e.g. EMP001"
                      className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                    />
                    <p className="text-xs text-gray-500 mt-1">Must match an existing employee's ID in the system.</p>
                  </div>
                  
                  <div className="pt-4 flex justify-end gap-3">
                    <button type="button" onClick={() => setIsModalOpen(false)} className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none">
                      Cancel
                    </button>
                    <button type="submit" className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-cbe-purple hover:bg-cbe-dark-purple focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-cbe-purple">
                      Register User
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
