import { useState } from 'react';
import api from '../../api/axios';

export default function EditProfile({ user, onClose, onUpdate }) {
  const [formData, setFormData] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    email: user?.email || ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setError('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    // Store original data for rollback if needed
    const originalData = { ...user };

   
    onUpdate({
      ...user,
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email
    });

    try {
      await api.put('/auth/update', formData);
      // Success 
      setTimeout(() => onClose(), 1000);
    } catch (err) {
      // ROLLBACK
      onUpdate(originalData);
      setError(err.response?.data?.message || 'Failed to update profile');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="security-section">
      <div className="section-header">
        <h2>Edit Profile</h2>
        <button className="btn-icon" onClick={onClose}>✕</button>
      </div>
      
      {error && <div className="alert-error">{error}</div>}
      
      <form onSubmit={handleSubmit} className="profile-form">
        <div className="form-row">
          <div className="form-group">
            <label>First Name</label>
            <input
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              required
              disabled={loading}
              placeholder="Enter first name"
              autoFocus
            />
          </div>
          
          <div className="form-group">
            <label>Last Name</label>
            <input
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              required
              disabled={loading}
              placeholder="Enter last name"
            />
          </div>
        </div>
        
        <div className="form-group">
          <label>Email Address</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
            disabled={loading}
            placeholder="Enter email address"
          />
        </div>

        <div className="form-actions">
          <button type="button" className="btn-outline" onClick={onClose} disabled={loading}>
            Cancel
          </button>
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Saving...' : 'Save Changes'}
          </button>
        </div>
      </form>
    </div>
  );
}

