import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import API from '../services/api'

function Login() {
  const navigate = useNavigate()

  const [formData, setFormData] = useState({
    email: '',
    password: ''
  })

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    })
  }

  const handleLogin = async (e) => {
    e.preventDefault()

    try {
      const response = await API.post('/auth/login', formData)

      localStorage.setItem('user', JSON.stringify(response.data))

      alert('Login Successful')

      navigate('/dashboard')
    } catch (error) {
      console.log(error)
      alert('Login Failed')
    }
  }

  return (
    <div className='container mt-5'>
      <div className='row justify-content-center'>
        <div className='col-md-4'>
          <div className='card p-4 shadow'>
            <h2 className='text-center mb-4'>Login</h2>

            <form onSubmit={handleLogin}>
              <input
                type='email'
                name='email'
                placeholder='Enter Email'
                className='form-control mb-3'
                onChange={handleChange}
              />

              <input
                type='password'
                name='password'
                placeholder='Enter Password'
                className='form-control mb-3'
                onChange={handleChange}
              />

              <button className='btn btn-primary w-100'>
                Login
              </button>
            </form>

            <p className='mt-3 text-center'>
              Don&apos;t have account?
              <Link to='/register'> Register</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Login