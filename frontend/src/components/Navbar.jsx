import { Link } from 'react-router-dom'

function Navbar() {
  return (
    <nav className='navbar navbar-dark bg-dark px-4'>
      <h3 className='text-white'>CTMS</h3>

      <div>
        <Link to='/dashboard' className='btn btn-light me-2'>
          Dashboard
        </Link>

        <Link to='/routes' className='btn btn-light me-2'>
          Routes
        </Link>

        <Link to='/buses' className='btn btn-light me-2'>
          Buses
        </Link>

        <Link to='/' className='btn btn-danger'>
          Logout
        </Link>
      </div>
    </nav>
  )
}

export default Navbar