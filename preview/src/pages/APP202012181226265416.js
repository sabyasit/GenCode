import React, {useCallback, useState} from 'react'
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import InputLabel from '@material-ui/core/InputLabel';
import FormHelperText from '@material-ui/core/FormHelperText';
import Button from '@material-ui/core/Button';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';
const useStyles = makeStyles((theme) => ({ root: { flexGrow: 1, padding: 10 }, rightMargin: { marginRight: 100 }, rightFloat: { float: 'right', top: -40 }, rightMarginPos: { paddingRight: 100, position: 'relative' }, rightFloatPos: { position: 'absolute', right: 0, top: 24 }, container: {  maxHeight: 440 } }));
function APP202012181226265416() {
const classes = useStyles();
const [body_id, Set_body_id] = useState('')
const [body_name, Set_body_name] = useState('')
const [body_photoUrls, Set_Array_body_photoUrls] = useState([''])
const [body_category_id, Set_body_category_id] = useState(0)
const [body_category_name, Set_body_category_name] = useState('')
const [body_tags, Set_body_tags] = useState([])
const [body_status, Set_body_status] = useState('')
const [response, Set_response] = useState('')
function Set_body_photoUrls(val, index)
{
body_photoUrls[index]=val;
Set_Array_body_photoUrls([...body_photoUrls]);
}
function Add_body_photoUrls()
{
Set_Array_body_photoUrls([...body_photoUrls, '']);
}
function Remove_body_photoUrls(index)
{
body_photoUrls.splice(index,1);
Set_Array_body_photoUrls([...body_photoUrls]);
}

const onSubmit = useCallback((event) => {
event.preventDefault();
const requestOptions = {
method: 'POST',
headers: { 'Content-Type': 'application/json' },
body: JSON.stringify({id: body_id,category:{id: body_category_id,name: body_category_name,},name: body_name,photoUrls: body_photoUrls,tags: body_tags,status: body_status,})
};
fetch('https://petstore.swagger.io/v2/pet', requestOptions)
.then(response => response.json())
.then(data => Set_response(data))
.catch (err => console.log(err));
})

return (
<div>
<form className={classes.root} autoComplete='off' onSubmit={onSubmit}>
<h1>Add a new pet to the store</h1>
<Grid container spacing={3}>
<Grid item xs={12}>
<TextField label='id' helperText='' type='number'  onInput={ e=>Set_body_id(e.target.value)} fullWidth />
</Grid>

<Grid item xs={12}>
<TextField label='name' helperText='' type='text' required onInput={ e=>Set_body_name(e.target.value)} fullWidth />
</Grid>

<Grid item xs={12}>
<Grid item xs={12}>
<Grid item xs={12} className={classes.rightMargin}>
<TextField label='photoUrls' helperText='' type='text' required onInput={ e=>Set_body_photoUrls(e.target.value, 0)} fullWidth />
</Grid>
<Button variant='contained' className={classes.rightFloat} onClick={Add_body_photoUrls}>Add</Button>
</Grid>
{body_photoUrls.map((value, index) => {
if (index > 0) {
return (
<Grid key={`tag-${index}`} item xs={12}>
<Grid item xs={12} className={classes.rightMargin}>
<TextField label='photoUrls' type='text' onInput={ e=>Set_body_photoUrls(e.target.value, index)} fullWidth />
</Grid>
<Button variant='contained' className={classes.rightFloat} onClick={e=> Remove_body_photoUrls(index)}>Remove</Button>
</Grid>
)
}
})}
</Grid>

<Grid item xs={12}><Button variant='contained' color='primary' type='submit'>Submit</Button></Grid>

</Grid>
</form>
<div style={{ padding: '10px' }}>
<Grid container spacing={3}>
</Grid>

</div>
</div>
);
}
export default APP202012181226265416;
