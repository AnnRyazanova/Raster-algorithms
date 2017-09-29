#include <iostream>
#include <cstdlib>

using namespace std;

#include <opencv\cv.h>
#include <opencv\highgui.h>

#include <fstream>

int main()
{
	IplImage* image = cvLoadImage("img.png", 1);
	assert(image != 0);

	IplImage* gray = cvCreateImage(cvGetSize(image), IPL_DEPTH_8U, 1);
	IplImage* bin = cvCreateImage(cvGetSize(image), IPL_DEPTH_8U, 1);

	IplImage* dst = cvCloneImage(image);

	cvCvtColor(image, gray, CV_RGB2GRAY);

	cvInRangeS(gray, cvScalar(40), cvScalar(150), bin);

	CvMemStorage* storage = cvCreateMemStorage(0);
	CvSeq* contours = 0;

	int contoursCont = cvFindContours(bin, storage, &contours, sizeof(CvContour), CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE, cvPoint(0, 0));

	double perimT = 0;
	CvSeq* seqT = 0;



	for (CvSeq* seq0 = contours; seq0 != 0; seq0 = seq0->h_next)
	{
		double perim = cvContourPerimeter(seq0);
		if (perim>perimT)
		{
			perimT = perim;
			seqT = seq0;
		}
	}


	/*cvDrawContours(dst, contours->h_next->h_next->h_next->h_next, CV_RGB(0, 0, 0), CV_RGB(0, 0, 250), 0, 1, 8); // рисуем контур

	cvNamedWindow("contours", CV_WINDOW_AUTOSIZE);
	cvShowImage("contours", dst);
	cvWaitKey(0);*/

	ofstream fout1("max_contour.txt");
	for (int i = 0; i<seqT->total; ++i)
	{
		CvPoint* p = (CvPoint*)cvGetSeqElem(seqT, i);
		fout1 << p->x << ", " << p->y << endl;
	}
	fout1.close();



	int i = 0;
	for (CvSeq* seq0 = contours; seq0 != contours->h_next->h_next->h_next->h_next; seq0 = seq0->h_next)
	{
		if (seq0 != seqT)
		{
			i++;
			ofstream fout2("contour" + to_string(i) + ".txt");
			for (int i = 0; i<seq0->total; ++i)
			{
				CvPoint* p = (CvPoint*)cvGetSeqElem(seq0, i);
				fout2 << p->x << ", " << p->y << endl;
			}
			fout2.close();
		}
	}



	cvReleaseImage(&image);
	cvReleaseImage(&gray);
	cvReleaseImage(&bin);
	cvReleaseImage(&dst);

	return 0;
}