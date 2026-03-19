import { useState } from 'react'
import HeadBar from '../components/HeadBar'
import Navbar from '../components/Navbar'
import Recipes from '../components/Recipes'
import type { RecipeType } from '../types/RecipeTypes'

const Home = () => {

  const [searchResults, setSearchResults] = useState<RecipeType[]>([])

  return (
    <>
      <Navbar/>
      <HeadBar onSearch={setSearchResults}/>
      <Recipes searchResults={searchResults}/>
    </>
  )
}

export default Home