import { useState, useEffect } from 'react';
import api from '../../api/axios';
import toast from 'react-hot-toast';
import { Plus, Edit2, Trash2, X, Upload } from 'lucide-react';

export default function Employees() {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [imageFile, setImageFile] = useState(null);
  const [imagePreview, setImagePreview] = useState(null);
  
  const [formData, setFormData] = useState({
    employeeId: '',
    fullName: '',
    division: '',
    department: '',
    jobTitle: '',
    phoneNumber: ''
  });

  const fetchEmployees = async () => {
    try {
      const response = await api.get('/employees');
      setEmployees(response.data.data || []);
    } catch (error) {
      toast.error('Failed to load employees');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEmployees();
  }, []);

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const openModal = (employee = null) => {
    setImageFile(null);
    setImagePreview(null);
    if (employee) {
      setEditingId(employee.id);
      setFormData({
        employeeId: employee.employeeId,
        fullName: employee.fullName,
        division: employee.division || '',
        department: employee.department || '',
        jobTitle: employee.jobTitle || '',
        phoneNumber: employee.phoneNumber || ''
      });
    } else {
      setEditingId(null);
      setFormData({
        employeeId: '',
        fullName: '',
        division: '',
        department: '',
        jobTitle: '',
        phoneNumber: ''
      });
    }
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingId(null);
    setImageFile(null);
    setImagePreview(null);
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setImageFile(file);
      setImagePreview(URL.createObjectURL(file));
    } else {
      setImageFile(null);
      setImagePreview(null);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      let response;
      if (editingId) {
        response = await api.put(`/employees/${editingId}`, formData);
      } else {
        const submitData = new FormData();
        Object.keys(formData).forEach(key => {
          submitData.append(key, formData[key]);
        });
        if (imageFile) {
          submitData.append('Image', imageFile);
        }
        
        response = await api.post('/employees/register', submitData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });
      }
      
      if (response.data.success !== false) {
        toast.success(editingId ? 'Employee updated successfully' : 'Employee registered successfully');
        closeModal();
        fetchEmployees();
      } else {
        toast.error(response.data.message || 'Error saving employee');
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Error saving employee');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this employee?')) {
      try {
        const response = await api.delete(`/employees/${id}`);
        if (response.data.success !== false) {
          toast.success('Employee deleted successfully');
          fetchEmployees();
        } else {
          toast.error(response.data.message || 'Failed to delete employee');
        }
      } catch (error) {
        toast.error('Failed to delete employee');
      }
    }
  };

  return (
    <div className="space-y-6">
      <div className="sm:flex sm:items-center sm:justify-between">
        <div>
          <h2 className="text-lg font-medium text-gray-900">Employees</h2>
          <p className="mt-1 text-sm text-gray-500">
            A list of all employees eligible for meeting attendance.
          </p>
        </div>
        <div className="mt-4 sm:mt-0">
          <button
            onClick={() => openModal()}
            className="flex items-center justify-center rounded-md border border-transparent bg-cbe-purple px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-cbe-dark-purple focus:outline-none focus:ring-2 focus:ring-cbe-purple focus:ring-offset-2 sm:w-auto"
          >
            <Plus className="mr-2 h-4 w-4" />
            Add Employee
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="bg-white shadow rounded-lg border border-gray-100 overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading employees...</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Employee</th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Division / Dept</th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Contact</th>
                  <th scope="col" className="relative px-6 py-3"><span className="sr-only">Actions</span></th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {employees.map((person) => (
                  <tr key={person.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm font-medium text-gray-900">{person.fullName}</div>
                      <div className="text-sm text-gray-500">{person.employeeId} &bull; {person.jobTitle}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900">{person.division || '-'}</div>
                      <div className="text-sm text-gray-500">{person.department || '-'}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {person.phoneNumber || '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button onClick={() => openModal(person)} className="text-cbe-purple hover:text-cbe-dark-purple mr-4">
                        <Edit2 size={16} />
                      </button>
                      <button onClick={() => handleDelete(person.id)} className="text-red-600 hover:text-red-900">
                        <Trash2 size={16} />
                      </button>
                    </td>
                  </tr>
                ))}
                {employees.length === 0 && (
                  <tr>
                    <td colSpan="4" className="px-6 py-10 text-center text-gray-500">
                      No employees found. Add one to get started.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
          <div className="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
            <div className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" onClick={closeModal}></div>
            <span className="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
            <div className="relative z-10 inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
              <div className="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4 border-t-4 border-cbe-purple">
                <div className="flex justify-between items-center mb-4">
                  <h3 className="text-lg leading-6 font-medium text-gray-900" id="modal-title">
                    {editingId ? 'Edit Employee' : 'Add New Employee'}
                  </h3>
                  <button onClick={closeModal} className="text-gray-400 hover:text-gray-500">
                    <X size={20} />
                  </button>
                </div>
                
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div className="col-span-2 sm:col-span-1">
                      <label className="block text-sm font-medium text-gray-700">Employee ID <span className="text-red-500">*</span></label>
                      <input type="text" name="employeeId" required value={formData.employeeId} onChange={handleInputChange} disabled={!!editingId}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm disabled:bg-gray-100" 
                      />
                    </div>
                    <div className="col-span-2 sm:col-span-1">
                      <label className="block text-sm font-medium text-gray-700">Full Name <span className="text-red-500">*</span></label>
                      <input type="text" name="fullName" required value={formData.fullName} onChange={handleInputChange}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                      />
                    </div>
                    <div className="col-span-2 sm:col-span-1">
                      <label className="block text-sm font-medium text-gray-700">Job Title</label>
                      <input type="text" name="jobTitle" value={formData.jobTitle} onChange={handleInputChange}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                      />
                    </div>
                    <div className="col-span-2 sm:col-span-1">
                      <label className="block text-sm font-medium text-gray-700">Phone Number</label>
                      <input type="text" name="phoneNumber" value={formData.phoneNumber} onChange={handleInputChange}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                      />
                    </div>
                    <div className="col-span-2 sm:col-span-1">
                      <label className="block text-sm font-medium text-gray-700">Division</label>
                      <input type="text" name="division" value={formData.division} onChange={handleInputChange}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                      />
                    </div>
                    <div className="col-span-2 sm:col-span-1">
                      <label className="block text-sm font-medium text-gray-700">Department</label>
                      <input type="text" name="department" value={formData.department} onChange={handleInputChange}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-cbe-purple focus:border-cbe-purple sm:text-sm" 
                      />
                    </div>
                    {!editingId && (
                      <div className="col-span-2">
                        <label className="block text-sm font-medium text-gray-700">Photo</label>
                        <div className="mt-1 flex items-center gap-4">
                          <div className="h-16 w-16 rounded-full overflow-hidden bg-gray-100 flex items-center justify-center border border-gray-300 shrink-0">
                            {imagePreview ? (
                              <img src={imagePreview} alt="Preview" className="h-full w-full object-cover" />
                            ) : (
                              <Upload className="h-6 w-6 text-gray-400" />
                            )}
                          </div>
                          <input
                            type="file"
                            accept="image/*"
                            onChange={handleImageChange}
                            className="block w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-cbe-purple file:text-white hover:file:bg-cbe-dark-purple"
                          />
                        </div>
                      </div>
                    )}
                  </div>
                  
                  <div className="pt-4 flex justify-end gap-3">
                    <button type="button" onClick={closeModal} className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none">
                      Cancel
                    </button>
                    <button type="submit" className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-cbe-purple hover:bg-cbe-dark-purple focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-cbe-purple">
                      Save Employee
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
