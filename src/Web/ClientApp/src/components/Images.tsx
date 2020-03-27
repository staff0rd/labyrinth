import * as React from 'react';
import { useEffect, useState } from 'react';
import { Paged } from '../store/Paged';
import { Image } from '../store/Image';
import Paper from '@material-ui/core/Paper';
import TablePagination from '@material-ui/core/TablePagination';
import { useSelector } from '../store/useSelector';
import { AccountState } from '../store/Account';
import { makeStyles } from '@material-ui/core/styles';
import { postResponse } from '../api'
import Alert from '@material-ui/lab/Alert';
import { useSource } from './useSource';
import { Card, CardActionArea, CardMedia, CardActions, Button, Grid } from '@material-ui/core';

type ImagesProps = {
  url: string;
  network: string;
};

const useStyles = makeStyles({
  root: {
    flex: 1,
  },
  media: {
    height: 200,
    '&.MuiCardMedia-root': {
      backgroundSize: 'contain',
    },
  },
});

export const Images = (props: ImagesProps) => {
  const {
    url,
    network,
  } = props;
  const classes = useStyles();
  const [error, setError] = useState<string>("");
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const { password, userName } = useSelector<AccountState>(state => state.account);
  const [images, setImages] = useState<Paged<Image>>();
  const { sourceId } = useSource(props.network);

  const searchRequest = () => {
    setError('');  
    postResponse<Paged<Image>>(url, {userName, password, pageNumber, pageSize, network, sourceId})
      .then(data => {
        if (data) {
          if (data.isError)
            setError(data.message!);
          else {
            setImages(data.response);
          }
        }
      });
    };

  useEffect(() => {
    searchRequest();
  }, [pageNumber, pageSize]);

  const convertImageUrl = (url: string) => {
    return url.replace('$labyrinth-image', '/api/events/image')
  }

  return (
    <>
      { error && <Alert severity="error">{error}</Alert> }
      { images && (
        <>
          <Grid container alignItems="stretch" spacing={1} style={{marginBottom: 8}}>
            { images.rows.map(image => (
              <Grid xs={12} sm={6} item key={image.id} style={{display: 'flex'}}>
                <Card className={classes.root}>
                  <CardActionArea>
                    <CardMedia
                      className={classes.media}
                      image={convertImageUrl(image.url)}
                      title=""
                    />
                  </CardActionArea>
                  <CardActions>
                    <Button size="small" color="primary">
                      Share
                    </Button>
                    <Button size="small" color="primary">
                      Learn More
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            ))}
          </Grid>
          <TablePagination 
            count={images.totalRows}
            rowsPerPage={images.pageSize}
            rowsPerPageOptions={[20, 50, 100]}
            component={Paper}
            onChangePage={(_: any, page: number) => setPageNumber(page)}
            onChangeRowsPerPage={((_: any, select: any) => setPageSize(select.key)) as any}
            page={images.page}
          />
        </>
      )}
    </>);
};
