import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../api/axios';
import '../App.css';

export default function Register() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const passwordStrength = (p) => {
    if (!p) return { score: 0, label: '', color: '' };
    if (p.length < 6) return { score: 1, label: 'Too short', color: '#ff6584' };
    if (p.length < 8) return { score: 2, label: 'Weak', color: '#f5a623' };
    if (/[A-Z]/.test(p) && /[0-9]/.test(p)) return { score: 4, label: 'Strong', color: '#43e97b' };
    return { score: 3, label: 'Good', color: '#43e97b' };
  };

  const strength = passwordStrength(form.password);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await api.post('/auth/register', form);
      navigate('/login');
    } catch (err) {
      setError(err.response?.data?.message || 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-screen">
      <div className="glow glow-top" />
      <div className="glow glow-bottom" />

      <div className="auth-card">
        <div className="card-eyebrow">Get started</div>
        <h1 className="card-title">Create your<br />account.</h1>
        <p className="card-sub">
          Already have an account?{' '}
          <Link to="/login" className="link">Sign in</Link>
        </p>

        {error && <div className="alert-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label className="form-label">First Name</label>
              <input
                className="form-input"
                name="firstName"
                type="text"
                placeholder="Sarah"
                value={form.firstName}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label className="form-label">Last Name</label>
              <input
                className="form-input"
                name="lastName"
                type="text"
                placeholder="Johnson"
                value={form.lastName}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label className="form-label">Email Address</label>
            <div className="input-icon-wrapper">
              <input
                className="form-input"
                name="email"
                type="email"
                placeholder="you@example.com"
                value={form.email}
                onChange={handleChange}
                required
              />
              <span className="input-icon">✉</span>
            </div>
          </div>

          <div className="form-group">
            <label className="form-label">Password</label>
            <div className="input-icon-wrapper">
              <input
                className="form-input"
                name="password"
                type="password"
                placeholder="Min. 8 characters"
                value={form.password}
                onChange={handleChange}
                required
              />
              <span className="input-icon">◉</span>
            </div>
            {form.password && (
              <>
                <div className="strength-bar">
                  {[1, 2, 3, 4].map((i) => (
                    <div
                      key={i}
                      className="strength-seg"
                      style={{ background: i <= strength.score ? strength.color : undefined }}
                    />
                  ))}
                </div>
                <div className="strength-label">
                  Password strength: <span style={{ color: strength.color }}>{strength.label}</span>
                </div>
              </>
            )}
          </div>

          <button className="btn-primary" type="submit" disabled={loading}>
            {loading ? 'Creating account…' : 'Create Account'}
          </button>
        </form>

        <div className="form-footer">
          By registering, you agree to our <a className="link">Terms</a> &amp;{' '}
          <a className="link">Privacy Policy</a>
        </div>
      </div>
    </div>
  );
}