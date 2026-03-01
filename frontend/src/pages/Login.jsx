import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../api/axios';
import '../App.css';

export default function Login() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const { data } = await api.post('/auth/login', form);
      sessionStorage.setItem('token', data.token);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid email or password.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-screen">
      <div className="glow glow-top" />
      <div className="glow glow-bottom" />

      <div className="auth-card">
        <div className="card-eyebrow">Welcome back</div>
        <h1 className="card-title">Sign in to your<br />account.</h1>
        <p className="card-sub">
          New here?{' '}
          <Link to="/register" className="link">Create an account →</Link>
        </p>

        {error && <div className="alert-error">{error}</div>}

        <form onSubmit={handleSubmit}>
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
                placeholder="Your password"
                value={form.password}
                onChange={handleChange}
                required
              />
              <span className="input-icon">◉</span>
            </div>
          </div>

          <div className="forgot">
            <a className="link">Forgot password?</a>
          </div>

          <button className="btn-primary" type="submit" disabled={loading}>
            {loading ? 'Signing in…' : 'Sign In'}
          </button>
        </form>

        <div className="jwt-note">
          <span>🔒</span>
          JWT token stored securely in session — cleared on tab close.
        </div>

        <div className="form-footer">
          Don't have an account? <Link to="/register" className="link">Register now</Link>
        </div>
      </div>
    </div>
  );
}