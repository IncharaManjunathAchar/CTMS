import { useState } from 'react'

import Navbar from '../components/Navbar'
import RouteCard from '../components/RouteCard'
import API from '../services/api'

function Routes() {
  const [source, setSource] = useState('')
  const [destination, setDestination] = useState('')
  const [routes, setRoutes] = useState([])

  const searchRoutes = async () => {
    try {
      const response = await API.get(
        `/route/search?source=${source}&destination=${destination}`
      )

      setRoutes(response.data)
    } catch (error) {
      console.log(error)
      alert('Failed to fetch routes')
    }
  }

  return (
    <div>
      <Navbar />

      <div className='container mt-5'>
        <div className='card p-4 shadow'>
          <h2>Search Routes</h2>

          <input
            type='text'
            placeholder='Enter Source'
            className='form-control mb-3'
            onChange={(e) => setSource(e.target.value)}
          />

          <input
            type='text'
            placeholder='Enter Destination'
            className='form-control mb-3'
            onChange={(e) => setDestination(e.target.value)}
          />

          <button
            className='btn btn-primary'
            onClick={searchRoutes}
          >
            Search
          </button>
        </div>

        <div className='mt-4'>
          {routes.map((route, index) => (
            <RouteCard key={index} route={route} />
          ))}
        </div>
      </div>
    </div>
  )
}

export default Routes