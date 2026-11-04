import Foundation
import AVFoundation
import AppKit
let args=CommandLine.arguments
let input=args[1], output=args[2]
let files=try FileManager.default.contentsOfDirectory(atPath:input).filter{$0.hasSuffix(".png")}.sorted()
guard let first=files.first,let image=NSImage(contentsOfFile:input+"/"+first), let cg=image.cgImage(forProposedRect:nil,context:nil,hints:nil) else {fatalError("No frames")}
let width=cg.width,height=cg.height
let url=URL(fileURLWithPath:output)
if FileManager.default.fileExists(atPath:output){try FileManager.default.removeItem(at:url)}
let writer=try AVAssetWriter(outputURL:url,fileType:.mp4)
let video=AVAssetWriterInput(mediaType:.video,outputSettings:[AVVideoCodecKey:AVVideoCodecType.h264,AVVideoWidthKey:width,AVVideoHeightKey:height,AVVideoCompressionPropertiesKey:[AVVideoAverageBitRateKey:4000000]])
video.expectsMediaDataInRealTime=false
let adaptor=AVAssetWriterInputPixelBufferAdaptor(assetWriterInput:video,sourcePixelBufferAttributes:[kCVPixelBufferPixelFormatTypeKey as String:kCVPixelFormatType_32ARGB,kCVPixelBufferWidthKey as String:width,kCVPixelBufferHeightKey as String:height,kCVPixelBufferCGImageCompatibilityKey as String:true,kCVPixelBufferCGBitmapContextCompatibilityKey as String:true])
writer.add(video);writer.startWriting();writer.startSession(atSourceTime:.zero)
for (i,file) in files.enumerated(){
 while !video.isReadyForMoreMediaData {Thread.sleep(forTimeInterval:0.005)}
 autoreleasepool{
  let img=NSImage(contentsOfFile:input+"/"+file)!.cgImage(forProposedRect:nil,context:nil,hints:nil)!
  var buffer:CVPixelBuffer?
  CVPixelBufferPoolCreatePixelBuffer(nil,adaptor.pixelBufferPool!,&buffer)
  let b=buffer!;CVPixelBufferLockBaseAddress(b,[])
  let context=CGContext(data:CVPixelBufferGetBaseAddress(b),width:width,height:height,bitsPerComponent:8,bytesPerRow:CVPixelBufferGetBytesPerRow(b),space:CGColorSpaceCreateDeviceRGB(),bitmapInfo:CGImageAlphaInfo.noneSkipFirst.rawValue)!
  context.draw(img,in:CGRect(x:0,y:0,width:width,height:height))
  CVPixelBufferUnlockBaseAddress(b,[])
  if !adaptor.append(b,withPresentationTime:CMTime(value:Int64(i),timescale:30)){fatalError("append failed")}
 }
}
video.markAsFinished()
let semaphore=DispatchSemaphore(value:0)
writer.finishWriting {semaphore.signal()};semaphore.wait()
guard writer.status == .completed else {fatalError(String(describing:writer.error))}
print("Wrote \(files.count) frames, \(Double(files.count)/30)s, \(width)x\(height): \(output)")
