import Navbar from '../components/Navbar'

function Dashboard() {
  const user = JSON.parse(localStorage.getItem('user'))

  return (
    <div>
      <Navbar />

      <div className='container mt-5'>
        <div className='card p-4 shadow'>
          <h2>Passenger Dashboard</h2>

          <hr />

          <h5>Name: {user?.name}</h5>

          <h5>Email: {user?.email}</h5>
        </div>
      </div>
    </div>
  )
}

export default Dashboard