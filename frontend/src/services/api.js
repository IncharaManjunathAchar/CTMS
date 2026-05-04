import axios from 'axios'

const API = axios.create({
  baseURL: 'https://localhost:7000/api'
})

export default API