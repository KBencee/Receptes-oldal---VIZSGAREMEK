import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import styles from '../css/Headbar.module.css'
import type { RecipeType } from '../types/RecipeTypes';
import { searchRecipesByTitle } from '../queryOptions/createRecipeQueryOption';
import { getTags } from '../services/postUpload';
import { useState } from 'react';


const HeadBar = ({ onSearch }: { onSearch: (results: RecipeType[]) => void }) => {
  let allTags: { cimkeNev: string; cimkeId: number }[] = [];
  getTags()
  
  const handleSearch = async (term: string) => {
    const [localSearchResults, setLocalSearchResults] = useState<RecipeType[]>([])
    const results = await searchRecipesByTitle(term)
    setLocalSearchResults(prev => [...prev, ...results])
    onSearch(results)
  }

  return (
    <div className={styles.headBar}>
        <h1>Receptek</h1>
        {/* <p>Ha nincs ötlet a főzéshez...<p>
        <button onClick={searchButtonClick}>Keresés...<FontAwesomeIcon icon={faMagnifyingGlass} /></button> */}
        <input onChange={(e) => handleSearch(e.target.value)} />
    </div>
  )
}

export default HeadBar