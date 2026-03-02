import { useEffect, useState } from 'react';
import { useNavigate, Routes, Route, Link } from 'react-router-dom';
import api from '../api/axios';
import '../App.css';
import ChangePassword from '../components/dashboard/ChangePassword';
import EditProfile from '../components/dashboard/EditProfile';




// Main Dashboard Component
export default function Dashboard() {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeSection, setActiveSection] = useState('profile');
  const [showEditProfile, setShowEditProfile] = useState(false);
  const [showChangePassword, setShowChangePassword] = useState(false);
  

  useEffect(() => {
    const token = sessionStorage.getItem('token');
    if (!token) { 
      navigate('/login'); 
      return; 
    }

    fetchUser();
  }, [navigate]);

  const fetchUser = async () => {
    try {
      const { data } = await api.get('/auth/me');
      setUser(data);
      // Update session activity
     // api.post('/auth/session/activity').catch(() => {});
    } catch (err) {
      sessionStorage.removeItem('token');
      navigate('/login');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = async () => {
    try {
      await api.post('/auth/logout');
    } catch (error) {
      // Ignore errors on logout
    } finally {
      sessionStorage.removeItem('token');
      navigate('/login');
    }
  };

  const handleProfileUpdate = (updatedUser) => {
    setUser(updatedUser);
  };

  const initials = user
    ? `${user.firstName?.[0] ?? ''}${user.lastName?.[0] ?? ''}`.toUpperCase()
    : '';

  const memberSince = user?.createdAt
    ? new Date(user.createdAt).toLocaleDateString('en-ZA', { 
        month: 'short', 
        year: 'numeric',
        day: 'numeric'
      })
    : '—';

  if (loading) {
    return (
      <div className="auth-screen" style={{ alignItems: 'center' }}>
        <div className="spinner" />
      </div>
    );
  }

  return (
    <div className="dashboard-wrapper">
      {/* ── Sidebar ── */}
      <aside className="sidebar">
        <nav className="sidebar-nav">
          <div className="sidebar-label">Navigation</div>
          <div 
            className={`sidebar-item ${activeSection === 'profile' && !showEditProfile && !showChangePassword ? 'active' : ''}`}
            onClick={() => {
              setActiveSection('profile');
              setShowEditProfile(false);
              setShowChangePassword(false);
            
            }}
          >
            <span className="s-icon">◈</span> Profile
          </div>
          
          <div className="sidebar-label">Security</div>
          <div 
            className={`sidebar-item ${showChangePassword ? 'active' : ''}`}
            onClick={() => {
              setShowChangePassword(true);
              setShowEditProfile(false);
            
              setActiveSection('');
            }}
          >
            <span className="s-icon">🔐</span> Change Password
          </div>
         
        </nav>

        <div className="sidebar-bottom">
          <div className="sidebar-avatar-row">
            <div className="avatar-sm">{initials}</div>
            <div>
              <div className="avatar-name">{user?.firstName} {user?.lastName}</div>
              <div className="avatar-role">Authenticated</div>
            </div>
          </div>
        </div>
      </aside>

      {/* ── Main ── */}
      <main className="main-content">
        {showEditProfile ? (
          <EditProfile 
            user={user} 
            onClose={() => setShowEditProfile(false)} 
            onUpdate={handleProfileUpdate}
          />
        ) : showChangePassword ? (
          <ChangePassword onClose={() => setShowChangePassword(false)} />
        ) 
         : (
          <>
            <div className="page-header">
              <div className="token-pill">
                <div className="token-dot" /> Bearer token active
              </div>
              <h1>My Profile</h1>
              <p>Your account information and activity.</p>
            </div>

            {/* Profile Hero */}
            <div className="profile-hero">
              <div className="avatar-lg">{initials}</div>
              <div className="profile-info">
                <h2>{user?.firstName} {user?.lastName}</h2>
                <div className="email">{user?.email}</div>
                <div className="badge-verified">✓ Email verified</div>
              </div>
              <div className="profile-actions">
                <button 
                  className="btn-outline"
                  onClick={() => setShowEditProfile(true)}
                >
                  Edit Profile
                </button>
                <button className="btn-danger" onClick={handleLogout}>Sign Out</button>
              </div>
            </div>

            {/* Detail Grid */}
            <div className="detail-grid">
              <div className="detail-card">
                <div className="dc-label"><span>👤</span> First Name</div>
                <div className="dc-value">{user?.firstName}</div>
              </div>
              <div className="detail-card">
                <div className="dc-label"><span>👤</span> Last Name</div>
                <div className="dc-value">{user?.lastName}</div>
              </div>
              <div className="detail-card">
                <div className="dc-label"><span>✉</span> Email Address</div>
                <div className="dc-value">{user?.email}</div>
              </div>
              <div className="detail-card">
                <div className="dc-label"><span>📅</span> Member Since</div>
                <div className="dc-value">{memberSince}</div>
              </div>
            </div>

            {/* Activity */}
            <div className="activity-card">
              <h3>Recent Activity</h3>
              <div className="activity-item">
                <div className="activity-dot green" />
                <div className="activity-text"><strong>Successful login</strong> — Session started</div>
                <div className="activity-time">Just now</div>
              </div>
              <div className="activity-item">
                <div className="activity-dot" />
                <div className="activity-text"><strong>Account created</strong> — Registration completed</div>
                <div className="activity-time">{memberSince}</div>
              </div>
            </div>
          </>
        )}
      </main>
    </div>
  );
}