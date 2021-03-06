﻿'***************************************************************************
' Project  : MultiWiiGUIControls                                            
' File     : MWGUIControl.vb                                         
' Version  : 1                                                              
' Language : C#                                                             
' Summary  : Generic class for the instrument control design                
' Creation : 15/06/2008                                                     
' Autor    : Original Controls written by Guillaume CHOUTEAU                
' History  :                                                                
'***************************************************************************


Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections
Imports System.Drawing
Imports System.Text
Imports System.Data


Namespace BaseflightGUIControls
    Class BaseflightControl
        Inherits System.Windows.Forms.Control
#Region "Generic methodes"

        ''' <summary>
        ''' Rotate an image on a point with a specified angle
        ''' </summary>
        ''' <param name="pe">The paint area event where the image will be displayed</param>
        ''' <param name="img">The image to display</param>
        ''' <param name="alpha">The angle of rotation in radian</param>
        ''' <param name="ptImg">The location of the left upper corner of the image to display in the paint area in nominal situation</param>
        ''' <param name="ptRot">The location of the rotation point in the paint area</param>
        ''' <param name="scaleFactor">Multiplication factor on the display image</param>
        Protected Sub RotateImage(pe As PaintEventArgs, img As Image, alpha As [Double], ptImg As Point, ptRot As Point, scaleFactor As Single)
            Dim beta As Double = 0
            ' Angle between the Horizontal line and the line (Left upper corner - Rotation point)
            Dim d As Double = 0
            ' Distance between Left upper corner and Rotation point)		
            Dim deltaX As Single = 0
            ' X componant of the corrected translation
            Dim deltaY As Single = 0
            ' Y componant of the corrected translation
            ' Compute the correction translation coeff
            If ptImg <> ptRot Then
                '
                If ptRot.X <> 0 Then
                    beta = Math.Atan(CDbl(ptRot.Y) / CDbl(ptRot.X))
                End If

                d = Math.Sqrt((ptRot.X * ptRot.X) + (ptRot.Y * ptRot.Y))

                ' Computed offset
                deltaX = CSng(d * (Math.Cos(alpha - beta) - Math.Cos(alpha) * Math.Cos(alpha + beta) - Math.Sin(alpha) * Math.Sin(alpha + beta)))
                deltaY = CSng(d * (Math.Sin(beta - alpha) + Math.Sin(alpha) * Math.Cos(alpha + beta) - Math.Cos(alpha) * Math.Sin(alpha + beta)))
            End If

            ' Rotate image support
            pe.Graphics.RotateTransform(CSng(alpha * 180 / Math.PI))

            ' Dispay image
            pe.Graphics.DrawImage(img, (ptImg.X + deltaX) * scaleFactor, (ptImg.Y + deltaY) * scaleFactor, img.Width * scaleFactor, img.Height * scaleFactor)

            ' Put image support as found
            pe.Graphics.RotateTransform(CSng(-alpha * 180 / Math.PI))

        End Sub


        ''' <summary>
        ''' Translate an image on line with a specified distance and a spcified angle
        ''' </summary>
        ''' <param name="pe">The paint area event where the image will be displayed</param>
        ''' <param name="img">The image to display</param>
        '''<param name="deltaPx">The translation distance in pixel</param>
        ''' <param name="alpha">The angle of translation direction in radian</param>
        ''' <param name="ptImg">The location of the left upper corner of the image to display in the paint area in nominal situation</param>
        ''' <param name="scaleFactor">Multiplication factor on the display image</param>
        Protected Sub TranslateImage(pe As PaintEventArgs, img As Image, deltaPx As Integer, alpha As Single, ptImg As Point, scaleFactor As Single)
            ' Computed offset
            Dim deltaX As Single = CSng(deltaPx * (Math.Sin(alpha)))
            Dim deltaY As Single = CSng(-deltaPx * (Math.Cos(alpha)))

            ' Dispay image
            pe.Graphics.DrawImage(img, (ptImg.X + deltaX) * scaleFactor, (ptImg.Y + deltaY) * scaleFactor, img.Width * scaleFactor, img.Height * scaleFactor)
        End Sub


        ''' <summary>
        ''' Rotate an image an apply a translation on the rotated image and the display it
        ''' </summary>
        ''' <param name="pe">The paint area event where the image will be displayed</param>
        ''' <param name="img">The image to display</param>
        ''' <param name="alphaRot">The angle of rotation in radian</param>
        ''' <param name="alphaTrs">The angle of translation direction in radian, expressed in the rotated image coordonate system</param>
        ''' <param name="ptImg">The location of the left upper corner of the image to display in the paint area in nominal situation</param>
        ''' <param name="ptRot">The location of the rotation point in the paint area</param>
        ''' <param name="deltaPx">The translation distance in pixel</param>
        ''' <param name="scaleFactor">Multiplication factor on the display image</param>
        Protected Sub RotateAndTranslate(pe As PaintEventArgs, img As Image, alphaRot As [Double], alphaTrs As [Double], ptImg As Point, deltaPx As Integer, _
            ptRot As Point, scaleFactor As Single)
            Dim beta As Double = 0
            Dim d As Double = 0
            Dim deltaXRot As Single = 0
            Dim deltaYRot As Single = 0
            Dim deltaXTrs As Single = 0
            Dim deltaYTrs As Single = 0

            ' Rotation

            If ptImg <> ptRot Then
                ' Internals coeffs
                If ptRot.X <> 0 Then
                    beta = Math.Atan(CDbl(ptRot.Y) / CDbl(ptRot.X))
                End If

                d = Math.Sqrt((ptRot.X * ptRot.X) + (ptRot.Y * ptRot.Y))

                ' Computed offset
                deltaXRot = CSng(d * (Math.Cos(alphaRot - beta) - Math.Cos(alphaRot) * Math.Cos(alphaRot + beta) - Math.Sin(alphaRot) * Math.Sin(alphaRot + beta)))
                deltaYRot = CSng(d * (Math.Sin(beta - alphaRot) + Math.Sin(alphaRot) * Math.Cos(alphaRot + beta) - Math.Cos(alphaRot) * Math.Sin(alphaRot + beta)))
            End If

            ' Translation

            ' Computed offset
            deltaXTrs = CSng(deltaPx * (Math.Sin(alphaTrs)))
            deltaYTrs = CSng(-deltaPx * (-Math.Cos(alphaTrs)))

            ' Rotate image support
            pe.Graphics.RotateTransform(CSng(alphaRot * 180 / Math.PI))

            ' Dispay image
            '            pe.Graphics.DrawImage(img, (ptImg.X + deltaXRot + deltaXTrs) * scaleFactor, (ptImg.Y + deltaYRot + deltaYTrs) * scaleFactor, img.Width * scaleFactor, img.Height * scaleFactor);
            pe.Graphics.DrawImageUnscaled(img, CInt(Math.Truncate(ptImg.X + deltaXRot + deltaXTrs)), CInt(Math.Truncate(ptImg.Y + deltaYRot + deltaYTrs)), img.Width, img.Height)


            ' Put image support as found
            pe.Graphics.RotateTransform(CSng(-alphaRot * 180 / Math.PI))
        End Sub


        ''' <summary>
        ''' Display a Scroll counter image like on gas  pumps 
        ''' </summary>
        ''' <param name="pe">The paint area event where the image will be displayed</param>
        ''' <param name="imgBand">The band counter image to display with digts : 0|1|2|3|4|5|6|7|8|9|0</param>
        ''' <param name="nbOfDigits">The number of digits displayed by the counter</param>
        ''' <param name="counterValue">The value to dispay on the counter</param>
        ''' <param name="ptImg">The location of the left upper corner of the image to display in the paint area in nominal situation</param>
        ''' <param name="scaleFactor">Multiplication factor on the display image</param>
        Protected Sub ScrollCounter(pe As PaintEventArgs, imgBand As Image, nbOfDigits As Integer, counterValue As Integer, ptImg As Point, scaleFactor As Single)
            Dim indexDigit As Integer = 0
            Dim digitBoxHeight As Integer = CInt(imgBand.Height \ 11)
            Dim digitBoxWidth As Integer = imgBand.Width

            For indexDigit = 0 To nbOfDigits - 1
                Dim currentDigit As Integer
                Dim prevDigit As Integer
                Dim xOffset As Integer
                Dim yOffset As Integer
                Dim fader As Double

                currentDigit = CInt(Math.Truncate((counterValue / Math.Pow(10, indexDigit)) Mod 10))

                If indexDigit = 0 Then
                    prevDigit = 0
                Else
                    prevDigit = CInt(Math.Truncate((counterValue / Math.Pow(10, indexDigit - 1)) Mod 10))
                End If

                ' xOffset Computing
                xOffset = CInt(digitBoxWidth * (nbOfDigits - indexDigit - 1))

                ' yOffset Computing	
                If prevDigit = 9 Then
                    fader = 0.33
                    yOffset = CInt(Math.Truncate(-((fader + currentDigit) * digitBoxHeight)))
                Else
                    yOffset = CInt(-(currentDigit * digitBoxHeight))
                End If

                ' Display Image
                pe.Graphics.DrawImage(imgBand, (ptImg.X + xOffset) * scaleFactor, (ptImg.Y + yOffset) * scaleFactor, imgBand.Width * scaleFactor, imgBand.Height * scaleFactor)
            Next
        End Sub

        Protected Sub DisplayRoundMark(pe As PaintEventArgs, imgMark As Image, insControlMarksDefinition As InstrumentControlMarksDefinition, ptImg As Point, radiusPx As Integer, displayText As [Boolean], _
            scaleFactor As Single)
            Dim alphaRot As [Double]
            Dim textBoxLength As Integer
            Dim textPointRadiusPx As Integer
            Dim textBoxHeight As Integer = CInt(Math.Truncate(insControlMarksDefinition.fontSize * 1.1 / scaleFactor))
            Dim textPoint As New Point()
            Dim rotatePoint As New Point()
            Dim markFont As New Font("Arial", insControlMarksDefinition.fontSize)
            Dim markBrush As New SolidBrush(insControlMarksDefinition.fontColor)
            Dim markArray As InstrumentControlMarkPoint() = New InstrumentControlMarkPoint(2 + (insControlMarksDefinition.numberOfDivisions - 1)) {}

            ' Buid the markArray
            markArray(0).value = insControlMarksDefinition.minPhy
            markArray(0).angle = insControlMarksDefinition.minAngle
            markArray(markArray.Length - 1).value = insControlMarksDefinition.maxPhy
            markArray(markArray.Length - 1).angle = insControlMarksDefinition.maxAngle

            For index As Integer = 1 To insControlMarksDefinition.numberOfDivisions
                markArray(index).value = (insControlMarksDefinition.maxPhy - insControlMarksDefinition.minPhy) / (insControlMarksDefinition.numberOfDivisions + 1) * index + insControlMarksDefinition.minPhy
                markArray(index).angle = (insControlMarksDefinition.maxAngle - insControlMarksDefinition.minAngle) / (insControlMarksDefinition.numberOfDivisions + 1) * index + insControlMarksDefinition.minAngle
            Next

            ' Define the rotate point (center of the indicator)
            rotatePoint.X = CInt(Math.Truncate((Me.Width \ 2) / scaleFactor))
            rotatePoint.Y = rotatePoint.X

            ' Display mark array
            For Each markPoint As InstrumentControlMarkPoint In markArray
                ' pre computings
                alphaRot = (Math.PI / 2) - markPoint.angle
                textBoxLength = CInt(Math.Truncate(Convert.ToString(markPoint.value).Length * insControlMarksDefinition.fontSize * 0.8 / scaleFactor))
                textPointRadiusPx = CInt(Math.Truncate(radiusPx - 1.2 * imgMark.Height - 0.5 * textBoxLength))
                textPoint.X = CInt(Math.Truncate((textPointRadiusPx * Math.Cos(markPoint.angle) - 0.5 * textBoxLength + rotatePoint.X) * scaleFactor))
                textPoint.Y = CInt(Math.Truncate((-textPointRadiusPx * Math.Sin(markPoint.angle) - 0.5 * textBoxHeight + rotatePoint.Y) * scaleFactor))

                ' Display mark
                RotateImage(pe, imgMark, alphaRot, ptImg, rotatePoint, scaleFactor)

                ' Display text
                If displayText = True Then
                    pe.Graphics.DrawString(Convert.ToString(markPoint.value), markFont, markBrush, textPoint)
                End If
            Next
        End Sub


        ''' <summary>
        ''' Convert a physical value in an rad angle used by the rotate function
        ''' </summary>
        ''' <param name="phyVal">Physical value to interpol</param>
        ''' <param name="minPhy">Minimum physical value</param>
        ''' <param name="maxPhy">Maximum physical value</param>
        ''' <param name="minAngle">The angle related to the minumum value, in deg</param>
        ''' <param name="maxAngle">The angle related to the maximum value, in deg</param>
        ''' <returns>The angle in radian witch correspond to the physical value</returns>
        Protected Function InterpolPhyToAngle(phyVal As Single, minPhy As Single, maxPhy As Single, minAngle As Single, maxAngle As Single) As Single
            Dim a As Single
            Dim b As Single
            Dim y As Single
            Dim x As Single

            If phyVal < minPhy Then
                Return CSng(minAngle * Math.PI / 180)
            ElseIf phyVal > maxPhy Then
                Return CSng(maxAngle * Math.PI / 180)
            Else

                x = phyVal
                a = (maxAngle - minAngle) / (maxPhy - minPhy)
                b = CSng(0.5 * (maxAngle + minAngle - a * (maxPhy + minPhy)))
                y = a * x + b

                Return CSng(y * Math.PI / 180)
            End If
        End Function

        Protected Function FromCartRefToImgRef(cartPoint As Point) As Point
            Dim imgPoint As New Point()
            imgPoint.X = cartPoint.X + (Me.Width \ 2)
            imgPoint.Y = -cartPoint.Y + (Me.Height \ 2)
            Return (imgPoint)
        End Function

        Protected Function FromDegToRad(degAngle As Double) As Double
            Dim radAngle As Double = degAngle * Math.PI / 180
            Return radAngle
        End Function


#End Region
    End Class

    Structure InstrumentControlMarksDefinition
        Public Sub New(myMinPhy As Single, myMinAngle As Single, myMaxPhy As Single, myMaxAngle As Single, myNumberOfDivisions As Integer, myFontSize As Integer, _
            myFontColor As Color, myScaleStyle As InstumentMarkScaleStyle)
            Me.minPhy = myMinPhy
            Me.minAngle = myMinAngle
            Me.maxPhy = myMaxPhy
            Me.maxAngle = myMaxAngle
            Me.numberOfDivisions = myNumberOfDivisions
            Me.fontSize = myFontSize
            Me.fontColor = myFontColor
            Me.scaleStyle = myScaleStyle
        End Sub
        Friend minPhy As Single
        Friend minAngle As Single
        Friend maxPhy As Single
        Friend maxAngle As Single
        Friend numberOfDivisions As Integer
        Friend fontSize As Integer
        Friend fontColor As Color
        Friend scaleStyle As InstumentMarkScaleStyle
    End Structure

    Structure InstrumentControlMarkPoint
        Public Sub New(myValue As Single, myAngle As Single)
            Me.value = myValue
            Me.angle = myAngle
        End Sub
        Friend value As Single
        Friend angle As Single
    End Structure

    Enum InstumentMarkScaleStyle
        Linear = 0
        Log = 1
    End Enum
End Namespace
