import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import API from '../services/api'

function Register() {
  const navigate = useNavigate()

  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: '',
    password: ''
  })

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    })
  }

  const handleRegister = async (e) => {
    e.preventDefault()

    try {
      await API.post('/auth/register', formData)

      alert('Registration Successful')

      navigate('/')
    } catch (error) {
      console.log(error)
      alert('Registration Failed')
    }
  }

  return (
    <div className='container mt-5'>
      <div className='row justify-content-center'>
        <div className='col-md-5'>
          <div className='card p-4 shadow'>
            <h2 className='text-center mb-4'>Register</h2>

            <form onSubmit={handleRegister}>
              <input
                type='text'
                name='name'
                placeholder='Enter Name'
                className='form-control mb-3'
                onChange={handleChange}
              />

              <input
                type='email'
                name='email'
                placeholder='Enter Email'
                className='form-control mb-3'
                onChange={handleChange}
              />

              <input
                type='text'
                name='phone'
                placeholder='Enter Phone'
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

              <button className='btn btn-success w-100'>
                Register
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Register