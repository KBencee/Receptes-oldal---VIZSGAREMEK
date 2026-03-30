import styles from '../css/Comments.module.css'
import type { CommentType } from '../types/RecipeTypes'

const Comment = (comment: CommentType) => {
  return (
	<>
		<div className={styles.comment}>
			<h3>{comment.username}:</h3>
			<p>{comment.szoveg}</p>
			<h6>{new Date(comment.irtaEkkor).toLocaleDateString()}</h6>
		</div>
	</>
  )
}

export default Comment