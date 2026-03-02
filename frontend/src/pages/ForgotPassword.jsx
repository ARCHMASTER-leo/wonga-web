import { useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/axios';

export default function ForgotPassword() {
  const [email, setEmail] = useState('');
  const [code, setCode] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [resetCode, setResetCode] = useState(null);
  const [step, setStep] = useState(1); // 1 = email, 2 = code + new password
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleRequestCode = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      const { data } = await api.post('/auth/forgot-password', { email });
      setResetCode(data.code); // show code on screen (dev only)
      setStep(2);
    } catch (err) {
      setError('Something went wrong.');
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      await api.post('/auth/reset-password', { email, code, newPassword });
      setMessage('Password reset successfully. You can now log in.');
      setStep(3);
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid or expired code.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-screen">
      <div className="glow glow-top" />
      <div className="glow glow-bottom" />
      <div className="auth-card">
        <h1 className="card-title">Forgot Password</h1>

        {error && <div className="alert-error">{error}</div>}
        {message && <div className="alert-success">{message}</div>}

        {step === 1 && (
          <form onSubmit={handleRequestCode}>
            <div className="form-group">
              <label className="form-label">Email Address</label>
              <input
                className="form-input"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <button className="btn-primary" type="submit" disabled={loading}>
              {loading ? 'Generating...' : 'Get Reset Code'}
            </button>
          </form>
        )}

        {step === 2 && (
          <form onSubmit={handleResetPassword}>
            {resetCode && (
              <div className="alert-success">
                🔑 Your reset code: <strong>{resetCode}</strong>
              </div>
            )}
            <div className="form-group">
              <label className="form-label">Reset Code</label>
              <input
                className="form-input"
                value={code}
                onChange={(e) => setCode(e.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label className="form-label">New Password</label>
              <input
                className="form-input"
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                required
              />
            </div>
            <button className="btn-primary" type="submit" disabled={loading}>
              {loading ? 'Resetting...' : 'Reset Password'}
            </button>
          </form>
        )}

        {step === 3 && (
          <Link to="/login" className="btn-primary">Back to Login</Link>
        )}

        <p className="card-sub" style={{ marginTop: '1rem' }}>
          <Link to="/login" className="link">Back to login</Link>
        </p>
      </div>
    </div>
  );
}